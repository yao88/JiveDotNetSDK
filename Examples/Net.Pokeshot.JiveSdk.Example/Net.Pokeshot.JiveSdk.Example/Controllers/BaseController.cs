using Net.Pokeshot.JiveSdk.Example.Util;
using Net.Pokeshot.JiveSdk.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Net.Pokeshot.JiveSdk.Example.Controllers
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
 
* The BaseController class extends the standard MVC Controller with methods to set 
* the language and user principal for the current thread processing the request
* 
*/
    public class BaseController : Controller
    {


        JiveInstance jiveInstanceForUser;
        bool isNewJiveInstance;
     
        public void SetAccess(string userId)
        {
            using (JiveSdkContext db = new JiveSdkContext())
            {
                try
                {


                    //Retrieve the current user from the database
                    //Not recommended for production where it would be more efficient
                    //to user a caching mechanism to minimize sql queries
                    var _myUser = db.Users
                                          .Include("JiveInstance")
                                          .Where(u => u.UserId == userId);
                    User myUser = null;

                    if (_myUser.Count() > 0)
                    {
                        myUser = _myUser.First();

                        if (myUser.DisplayName != null)
                        {
                            ViewBag.ownerName = myUser.DisplayName;
                        }
                        else
                        {
                            ViewBag.ownerName = "Display name not set in DB";

                        }

                        ViewBag.ownerId = userId;
                        ViewBag.jiveUrl = myUser.JiveInstance.Url;
                        string culture = Thread.CurrentThread.CurrentCulture.ToString();
 

                    }
                    else
                    {
                        //This user is not registered in the database yet

                        string[] jiveInstanceTemp = userId.Split('@');
                        String userJiveInstance = jiveInstanceTemp[1];



                        // Find the instance ID for the user
                        var _jiveInstanceForUser = db.JiveInstances
                            .Include("Users")
                            .Include("JiveInstanceSettings")
                            .Where(j => j.JiveInstanceId.Equals(userJiveInstance));

                        if (_jiveInstanceForUser.Count() == 0)
                        {
                            //The JiveInstance does not exist yet. This is a legacy setting for apps delivered via the Jive Apps market
                            isNewJiveInstance = true;
                            jiveInstanceForUser = null;
                        }
                        else
                        {
                            isNewJiveInstance = false;
                            jiveInstanceForUser = _jiveInstanceForUser.First();
                            if (jiveInstanceForUser.Users.Count() == 0)
                            {
                                jiveInstanceForUser.Users = new List<User>();

                            }
                        }



                        //Create a new JiveInstance if no one has installed the app on this system yet
                        //This is a legacy setting for apps delivered via the Jive Apps market
                        if (isNewJiveInstance == true)
                        {

                            JiveInstance jiveInstance = new JiveInstance();
                            jiveInstance.JiveInstanceId = jiveInstanceTemp[1];
                            jiveInstance.DateCreated = DateTime.Now;
                            jiveInstance.LastUpdated = DateTime.Now;
                            jiveInstance.IsComplete = false;
                            jiveInstance.Users = new List<User>();
                            jiveInstance.IsLicensed = true;
                            jiveInstance.IsInstalledViaAddon = true;
                            db.JiveInstances.Add(jiveInstance);
                            db.SaveChanges();
                            jiveInstanceForUser = jiveInstance;

                        }



                        User user = new User();
                        user.UserId = userId;
                        user.JiveInstance = jiveInstanceForUser;
                        user.DateCreated = DateTime.Now;
                        user.LastUpdated = DateTime.Now;
                        user.HasInstalledApp = true;
                        
                        db.Users.Add(user);
                        db.SaveChanges();
                        jiveInstanceForUser.Users.Add(user);
                        db.SaveChanges();

                        ViewBag.jiveUrl = user.JiveInstance.Url;
                        ViewBag.ownerName = "Display name not set in DB";
                        ViewBag.ownerId = user.UserId;

                    }


                    //Setting some helper data into the viewbag, so we can efficiently build a page

                    //The baseUrl is the system on which our addon is hosted. We need this to set the base_href for our html pages correctly
                    string baseUrl = System.Configuration.ConfigurationManager.AppSettings["baseUrl"];
                    ViewBag.baseUrl = baseUrl;

                    //The appPath is the path of our application, e.g. https://myjiveaddon.azurewebsites.net/apps/app_path
                    //We might need this to build deep links to our application
                    string appPath = System.Configuration.ConfigurationManager.AppSettings["appPath"];
                    ViewBag.appPath = appPath;

                    //Version information for app and included resources. Due to the caching mechanisms in Jive
                    //we recommend to update this when you roll out new versions
                    string jsVersion = System.Configuration.ConfigurationManager.AppSettings["jsVersion"];
                    ViewBag.jsVersion = jsVersion;
                    string appJsVersion = System.Configuration.ConfigurationManager.AppSettings["appJsVersion"];
                    ViewBag.appJsVersion = appJsVersion;


                    ViewBag.jiveUrl = myUser.JiveInstance.Url;

                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                  
                }
            }
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            //The Jive Add-On Framework stips out Accept-Language headers from requests
            //Instead a lang querystring is sent with every request
            //The reason for this is that a user might have set a different language in Jive
            //Than in their browser
            string languageCode = "en";


            if (requestContext.HttpContext.Request.QueryString != null)
            {
                int i = 0;
                var queryValues = requestContext.HttpContext.Request.QueryString;
                foreach (String key in queryValues)
                {
                    if (key.Equals("lang"))
                    {
                        languageCode = requestContext.HttpContext.Request.QueryString.GetValues(i)[0];
                    }
                    i++;
                }


                // Validate culture name
                string cultureName = CultureHelper.GetImplementedCulture(languageCode); // This is safe


                // Modify current thread's cultures            
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            }
            base.Initialize(requestContext);
        }
    }
}