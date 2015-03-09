using Net.Pokeshot.JiveSdk.Example.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Net.Pokeshot.JiveSdk.Example.Controllers
{
    public class HomeController : BaseController
    {
        [AuthorizeRequest]
        public ActionResult Index()
        {
            //The generic principal has been set by the SignedRequestModule HttpModule
            
            string ownerId = Thread.CurrentPrincipal.Identity.Name;

            SetAccess(ownerId);
            return View();
        }
        [AuthorizeRequest]
        public ActionResult ReadData()
        {
            //The generic principal has been set by the SignedRequestModule HttpModule

            string ownerId = Thread.CurrentPrincipal.Identity.Name;

            SetAccess(ownerId);
            return View();
        }
        [AuthorizeRequest]
        public ActionResult PostData()
        {
            //The generic principal has been set by the SignedRequestModule HttpModule

            string ownerId = Thread.CurrentPrincipal.Identity.Name;

            SetAccess(ownerId);
            return View();
        }

    }
}