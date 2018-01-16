using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DriftService
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Log", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Message",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Message", action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}
