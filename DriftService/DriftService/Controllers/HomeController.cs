using DriftService.Context;
using DriftService.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DriftService.Controllers
{
    public class HomeController : Controller
    {
        DriftContext db = new DriftContext();
        public ActionResult Index()
        {
            var logs = (from a in db.Logs
                        orderby a.LogID descending
                        select a).ToList().Take(10);

            return View(logs);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

       
    }
}