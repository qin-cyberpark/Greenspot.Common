using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Greenspot.WeChatAuth
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
             name: "Refresh",
             url: "Refresh",
             defaults: new { controller = "Home", action = "Refresh", id = UrlParameter.Optional }
            );

            routes.MapRoute(
               name: "Default",
               url: "{id}/{*url}",
               defaults: new { controller = "Home", action = "Callback", id = UrlParameter.Optional }
           );
        }
    }
}
