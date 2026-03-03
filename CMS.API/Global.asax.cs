using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Optimization;

namespace CMS.API
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            //if (!Request.IsSecureConnection && !Request.IsLocal)
            //{
            //    var path = $"https://{Request.Url.Host}:8443{Request.Url.PathAndQuery}";
            //    Response.Redirect(path);
            //    // 여기부터는 optional 
            //    Response.Status = "301 Moved Permanently";
            //    Response.AddHeader("Location", path);
            //}
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
