using Net.Pokeshot.JiveSdk.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Net.Pokeshot.JiveSdk.Example.Util
{
    public class AuthorizeRequest : AuthorizeAttribute
    {


        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            try
            {
                if (HttpContext.Current.Request.Headers["authorization"] != null)
                {

                    string authString = HttpContext.Current.Request.Headers["authorization"];
                    string userId = HttpContext.Current.Request.Headers["x-jive-user-id"];

                    string[] authStringArray = authString.Split('&');
                    string tenantId = null;
                    string jiveUrl = null;
                    foreach (string authElement in authStringArray)
                    {
                        string[] keyValue = authElement.Split('=');
                        if (keyValue[0].Equals("tenant_id"))
                        {
                            tenantId = keyValue[1];
                        }

                        if (keyValue[0].Equals("jive_url"))
                        {
                            jiveUrl = HttpUtility.UrlDecode(keyValue[1]);
                        }
                    }
                    string ownerId = userId + "@" + tenantId;

                    using (JiveSdkContext db = new JiveSdkContext())
                    {
                        User myUser = null;
                        var _myUser = db.Users
                            .Include("JiveInstance.Users")
                            .Where(u => u.UserId.Equals(ownerId));
                        if (_myUser.Count() > 0)
                        {
                            myUser = _myUser.First();
                        }
                        else
                        {
                            if (myUser == null)
                            {
                                string[] jiveInstanceTemp = ownerId.Split('@');
                                String userJiveInstance = jiveInstanceTemp[1];

                                JiveInstance jiveInstanceForUser = null;


                                // Find the instance ID for the user
                                var _jiveInstanceForUser = db.JiveInstances
                                    .Include("Users")
                                    .Where(j => j.JiveInstanceId.Equals(userJiveInstance));

                                jiveInstanceForUser = _jiveInstanceForUser.First();
                                if (jiveInstanceForUser.Users == null)
                                {
                                    jiveInstanceForUser.Users = new List<User>();

                                }


                                myUser = new User();
                                myUser.DateCreated = DateTime.Now;
                                myUser.HasInstalledApp = true;
                                myUser.IsComplete = false;
                                myUser.UserId = ownerId;
                                myUser.LastUpdated = DateTime.Now;
                                //
                                db.Users.Add(myUser);
                                myUser.JiveInstance = jiveInstanceForUser;
                                db.SaveChanges();
                                jiveInstanceForUser.Users.Add(myUser);




                                db.SaveChanges();
                            }
                        }

                        GenericIdentity MyIdentity = new GenericIdentity(ownerId);

                        String[] MyStringArray = { "User" };
                        GenericPrincipal MyPrincipal =
                            new GenericPrincipal(MyIdentity, MyStringArray);
                        Thread.CurrentPrincipal = MyPrincipal;
                        return true;


                    }
                }
                else
                {

                    return false;

                }

            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return false;
            }

        }
    }
}