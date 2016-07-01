using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Greenspot.WeChatAuth.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Callback(string id,string url)
        {
            
            if (string.IsNullOrEmpty(id))
            {
                return View();
            }

            var app = WeChatMpManager.Find(id);
            if (app == null)
            {
                return View();
            }

            //return Redirect(app.GetRedirectUrl(url, Request["code"], Request["state"]));
            return Redirect(app.GetRedirectUrl(url, Request.QueryString));
        }

        public ActionResult Refresh()
        {
            WeChatMpManager.Load();
            return View("List");
        }

        public ActionResult List()
        {
            return View();
        }
    }
}