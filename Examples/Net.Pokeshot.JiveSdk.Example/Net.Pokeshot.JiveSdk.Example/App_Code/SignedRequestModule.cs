using Net.Pokeshot.JiveSdk.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Threading;
using System.Web;

namespace Net.Pokeshot.JiveSdk.Example.App_Code
{

     /*
     * Jive Signed Request Validator for ASP.Net MCV
     * Copyright (c) SMZ SocialMediaZolutions GmbH & Co. KG
     * All rights reserved.
     *
     * MIT License
     * Permission is hereby granted, free of charge, to any person obtaining a copy of this
     * software and associated documentation files (the ""Software""), to deal in the Software
     * without restriction, including without limitation the rights to use, copy, modify, merge,
     * publish, distribute, sublicense, and/or sell copies of the Software, and to permit
     * persons to whom the Software is furnished to do so, subject to the following conditions:
     * The above copyright notice and this permission notice shall be included in all copies or
     * substantial portions of the Software.
     * THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
     * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
     * PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
     * FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
     * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
     * DEALINGS IN THE SOFTWARE.
 
     * This class can be used to validate the signature of a signed request originating from a registered Jive environment 
     * to a ASP.Net MCV Controller
     * 
     */

    public class SignedRequestModule : IHttpModule
    {


        private static readonly string PARAM_ALGORITHM = "algorithm";
        private static readonly string PARAM_CLIENT_ID = "client_id";
        private static readonly string PARAM_JIVE_URL = "jive_url";
        private static readonly string PARAM_TENANT_ID = "tenant_id";
        private static readonly string PARAM_TIMESTAMP = "timestamp";
        private static readonly string PARAM_SIGNATURE = "signature";

        private static readonly string JIVE_EXTN = "JiveEXTN ";
        private JiveSdkContext db = new JiveSdkContext();
        private Dictionary<string, string> GetParametersFromAuthHeader(string authHeader)
        {
            authHeader = authHeader.Substring(JIVE_EXTN.Length);
            string[] parameters = authHeader.Split('&', '?');
            Dictionary<string, string> parameterDictionary = new Dictionary<string, string>();
            foreach (string keyValue in parameters)
            {
                string[] tokens = keyValue.Split('=');
                if (tokens.Length != 2)
                {
                    Trace.WriteLine("Authorization header not formatted correctly");
                    throw new HttpRequestValidationException();
                }
                parameterDictionary.Add(HttpUtility.UrlDecode(tokens[0]), HttpUtility.UrlDecode(tokens[1]));

            }

            return parameterDictionary;
        }

        private string validateSignature(string parameterStrWithoutSignature, string clientSecret)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            //byte[] secret = encoding.GetBytes(clientSecret);
            byte[] secret = Convert.FromBase64String(clientSecret);


            byte[] headerToValidate = encoding.GetBytes(parameterStrWithoutSignature);
            HMACSHA256 hmacsha256 = new HMACSHA256(secret);

            byte[] calculatedSignature = hmacsha256.ComputeHash(headerToValidate);

            return Convert.ToBase64String(calculatedSignature);
        }

        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += OnBeginRequest;
            context.EndRequest += OnEndRequest;

        }


        void OnBeginRequest(object sender, System.EventArgs e)
        {

            try
            {
                bool oauthValidationEnabled = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsOauthValidationEnabled"]);

                if (oauthValidationEnabled)
                {
                    //Exclude WebApi requests from this validation
                    if (HttpContext.Current.Request.Url.AbsolutePath.StartsWith("/api") == false)
                    {
                     
                            if (HttpContext.Current.Request.Headers["authorization"] != null)
                            {



                                try
                                {


                                    string authHeader = HttpContext.Current.Request.Headers["authorization"];

                                    //string authString = HttpContext.Current.Request.Headers["authorization"];
                                    string userId = HttpContext.Current.Request.Headers["x-jive-user-id"];

                                    if (authHeader.StartsWith(JIVE_EXTN) == false || authHeader.Contains(PARAM_SIGNATURE) == false)
                                    {
                                        Trace.WriteLine("Authorization header not formatted correctly");
                                        throw new HttpRequestValidationException();
                                    }

                                    Dictionary<string, string> parameterDict = GetParametersFromAuthHeader(authHeader);
                                    string signature = parameterDict[PARAM_SIGNATURE];
                                    parameterDict.Remove(PARAM_SIGNATURE);
                                    string algorithm = parameterDict[PARAM_ALGORITHM];
                                    string clientId = parameterDict[PARAM_CLIENT_ID];
                                    string jiveUrl = parameterDict[PARAM_JIVE_URL];
                                    string tenantId = parameterDict[PARAM_TENANT_ID];
                                    string timeStampStr = parameterDict[PARAM_TIMESTAMP];

                                    long timestampMilliSeconds = Convert.ToInt64(timeStampStr);
                                    DateTime timestamp = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(timestampMilliSeconds);
                                    if (timestamp > DateTime.Now.AddMinutes(5) || DateTime.Now.AddMinutes(-5) > timestamp)
                                    {
                                        Trace.WriteLine("Timestamp older than 5 minutes");
                                        int timestampDiff = timestamp.CompareTo(DateTime.Now);
                                        throw new HttpRequestValidationException("Timestamp difference more than 5 minutes. Difference: " + timestampDiff.ToString());
                                    }

                                    var _myAddon = db.JiveAddons
                                        .Include("JiveInstance")
                                        .Where(a => a.JiveInstance.JiveInstanceId.Equals(tenantId));
                                    if (_myAddon.Count() == 0)
                                    {
                                        Trace.WriteLine("Jive Instance not found");
                                        throw new HttpRequestValidationException();
                                    }
                                    JiveAddon myAddon = _myAddon.Single();
                                    if (myAddon.ClientId.Equals(clientId) == false)
                                    {
                                        Trace.WriteLine("Not the expected client id for this tenant");
                                        throw new HttpRequestValidationException();
                                    }

                                    string parameterStrWithoutSignature = authHeader.Substring(JIVE_EXTN.Length, authHeader.IndexOf(PARAM_SIGNATURE) - (PARAM_SIGNATURE.Length + 1));


                                    string expectedSignature = validateSignature(parameterStrWithoutSignature, myAddon.ClientSecret);

                                    if (expectedSignature.Equals(signature))
                                    {
                                        string ownerId = userId + "@" + tenantId;

                                        GenericIdentity MyIdentity = new GenericIdentity(ownerId);

                                        String[] MyStringArray = { "User" };
                                        GenericPrincipal MyPrincipal =
                                            new GenericPrincipal(MyIdentity, MyStringArray);
                                        Thread.CurrentPrincipal = MyPrincipal;
                                    }
                                    else
                                    {
                                        Trace.WriteLine("Signature not correctly validated");
                                        throw new HttpRequestValidationException();
                                    }


                                }
                                catch (HttpRequestValidationException authEx)
                                {
                                    Trace.WriteLine(authEx.Message, "Error");


                                    HttpContext.Current.Response.Status = "401 Unauthorized";

                                    HttpContext.Current.Response.StatusCode = 401;
                                    HttpContext.Current.Response.End();

                                }




                            }
                            else
                            {

                                if (HttpContext.Current.Request.Url.AbsolutePath.Contains(".") == false)
                                {
                                    HttpContext.Current.Response.Status = "403 Forbidden";

                                    HttpContext.Current.Response.StatusCode = 403;
                                    HttpContext.Current.Response.End();
                                }               
                            }
                        



                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex, "Error");
                

            }

        }


        void OnEndRequest(object sender, System.EventArgs e)
        {

        }
    }
}