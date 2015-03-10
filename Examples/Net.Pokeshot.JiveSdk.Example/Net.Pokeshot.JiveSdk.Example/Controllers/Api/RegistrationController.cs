using AutoMapper;
using Net.Pokeshot.JiveSdk.Models;
using Net.Pokeshot.JiveSdk.Models.Dto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Net.Pokeshot.JiveSdk.Example.Controllers.Api
{
    /*
    * Jive Add-On Registration Controller
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
 
    * This class contains the RegisterJiveInstance Method, which takes a registration request from Jive Add-on
     * upon installation, uninstallation, or re-connect and creates/updates the Jive Instance
     * and Jive Add-On entries in the database accordingly
    * 
    */
    public class RegistrationController : ApiController
    {
        private JiveSdkContext db = new JiveSdkContext();
        private string _response = "";

        [HttpPost]
        public async Task<HttpResponseMessage> RegisterJiveInstance(JiveAddonRegistrationDto jiveRegistration)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _response = await Request.Content.ReadAsStringAsync();

                    //Convert our Jive Addon registration from the Data Transfer Object into an internal object
                    //Example of an add-on registration request
                    //{
                    //    "tenantId": "b22e3911-28ef-480c-ae3b-ca791ba86952",
                    //    "jiveSignatureURL": "https://market.apps.jivesoftware.com/appsmarket/services/rest/jive/instance/validation/8ce5c231-fab8-46b1-b8b2-fc65ascesb5d",
                    //    "timestamp": "2014-02-17T14:07:09.135+0000",
                    //    "jiveUrl": "https://sandbox.jiveon.com",
                    //    "jiveSignature": "r3gMujBUIWLUyvAp81TK9YasdAdDaRtlsQ6x+Ig=",
                    //    "clientSecret": "bmdoprg381uypaffd7xrl123c9znb5fjsb.s",
                    //    "clientId": "mrymd1f8oziyamo0yxdasdw9yovigd9t.i"
                    //}

                    Mapper.CreateMap<JiveAddonRegistrationDto, JiveAddonRegistration>();
                    JiveAddonRegistration myJiveRegistration = Mapper.Map<JiveAddonRegistrationDto, JiveAddonRegistration>(jiveRegistration);

                    db.JiveRegistrations.Add(myJiveRegistration);
                    db.SaveChanges();

                    if (Convert.ToBoolean(jiveRegistration.uninstalled) != true)
                    {
                        //check if this is a valid Jive environment

                        StringBuilder validationPayload = new StringBuilder();
                        validationPayload.Append("clientId:");
                        validationPayload.Append(myJiveRegistration.ClientId + "\n");
                        validationPayload.Append("clientSecret:");

                        System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                        byte[] secret = encoding.GetBytes(myJiveRegistration.ClientSecret);
                        HMACSHA256 hmacsha256 = new HMACSHA256();
                        hmacsha256.ComputeHash(secret);




                        foreach (byte b in hmacsha256.Hash)
                        {
                            validationPayload.AppendFormat("{0:x2}", b);
                        }
                        validationPayload.Append("\n" + "jiveSignatureURL:");
                        validationPayload.Append(myJiveRegistration.JiveSignatureURL + "\n");
                        validationPayload.Append("jiveUrl:");
                        validationPayload.Append(myJiveRegistration.JiveUrl + "\n");
                        validationPayload.Append("tenantId:");
                        validationPayload.Append(myJiveRegistration.TenantId + "\n");
                        validationPayload.Append("timestamp:");
                        validationPayload.Append(myJiveRegistration.Timestamp + "\n");
                        string validationPayloadString = validationPayload.ToString();
                        byte[] binaryPayload = encoding.GetBytes(validationPayloadString);

                        //Validate with the Jive webservice that the request orininated from JIve
                        //Alternatively validate with your local environment in some form
                        //This method is not fully implemented in this example
                        using (WebClient wc = new WebClient())
                        {
                            wc.Headers.Set("X-Jive-MAC", myJiveRegistration.JiveSignature);
                            try
                            {
                                byte[] postResult = wc.UploadData(myJiveRegistration.JiveSignatureURL, binaryPayload);
                            }
                            catch (System.Net.WebException webEx)
                            {
                                Trace.WriteLine(webEx, "Error");
                            }
                        }
                    }
                    //Check if the Jive instance already exists, otherwise create it

                    JiveInstance myJiveInstance = db.JiveInstances.Find(myJiveRegistration.TenantId);



                    if (myJiveInstance == null)
                    {
                        //Jive instance not created yet. Let's build it

                        myJiveInstance = new JiveInstance();
                        myJiveInstance.DateCreated = DateTime.Now;
                        myJiveInstance.IsComplete = false;
                        myJiveInstance.JiveInstanceId = myJiveRegistration.TenantId;
                        myJiveInstance.LastUpdated = DateTime.Now;
                        myJiveInstance.Url = myJiveRegistration.JiveUrl;
                        myJiveInstance.IsComplete = false;
                        myJiveInstance.IsLicensed = true;
                        myJiveInstance.IsInstalledViaAddon = true;
                        db.JiveInstances.Add(myJiveInstance);

                        db.SaveChanges();

                        User newAdmin = new User();
                        newAdmin.DateCreated = DateTime.Now;


                    }
                    JiveAddon myJiveAddon;
                    if (Convert.ToBoolean(myJiveRegistration.Uninstalled) == true)
                    {
                        //Addon uninstalled
                        myJiveAddon = db.JiveAddons
                            .Include("JiveInstance")
                            .Where(a => a.JiveInstance.JiveInstanceId.Equals(myJiveRegistration.TenantId)).First();
                        myJiveAddon.Uninstalled = true;
                        db.SaveChanges();

                    }
                    else
                    {
                        //Addon is being installed
                        //Create a new add-on
                        var _jiveAddons = db.JiveAddons
                            .Include("JiveInstance")
                            .Where(a => a.JiveInstance.JiveInstanceId.Equals(myJiveRegistration.TenantId));


                        if (_jiveAddons.Count() == 0)
                        {
                            myJiveAddon = new JiveAddon();
                            myJiveAddon.AddonType = "My wonderful app";
                            myJiveAddon.ClientId = myJiveRegistration.ClientId;
                            string clientSecret = myJiveRegistration.ClientSecret;
                            if (clientSecret.EndsWith(".s"))
                            {
                                clientSecret = clientSecret.TrimEnd('s');
                                clientSecret = clientSecret.TrimEnd('.');

                            }
                            myJiveAddon.ClientSecret = clientSecret;
                            myJiveAddon.DateCreated = DateTime.Now;
                            myJiveAddon.JiveInstance = myJiveInstance;
                            myJiveAddon.Code = myJiveRegistration.Code;
                            myJiveAddon.Scope = myJiveRegistration.Scope;
                            myJiveRegistration.Timestamp = myJiveRegistration.Timestamp;
                            myJiveAddon.Uninstalled = false;
                            db.JiveAddons.Add(myJiveAddon);
                            db.SaveChanges();
                        }
                        else
                        {
                            //Update existing addon

                            myJiveAddon = _jiveAddons.First();
                            myJiveAddon.ClientId = myJiveRegistration.ClientId;

                            string clientSecret = myJiveRegistration.ClientSecret;
                            if (clientSecret.EndsWith(".s"))
                            {
                                clientSecret = clientSecret.TrimEnd('s');
                                clientSecret = clientSecret.TrimEnd('.');

                            }
                            myJiveAddon.ClientSecret = clientSecret;
                            myJiveAddon.Code = myJiveRegistration.Code;
                            myJiveAddon.Scope = myJiveRegistration.Scope;
                            myJiveRegistration.Timestamp = myJiveRegistration.Timestamp;
                            myJiveAddon.Uninstalled = false;
                            db.SaveChanges();

                        }



                    }


                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, jiveRegistration);
                    return response;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex, "Error");
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.InternalServerError);
                    return response;
                }


            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }


    }
}
