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
        private DriftContext db = new DriftContext();
        private List<Log> listOfLogs = new List<Log>();
        private String StringForParsing = "";

        public ActionResult Index()
        {
            var logs = (from a in db.Logs
                        orderby a.LogID descending
                        select a).ToList().Take(10);

            foreach (var i in logs.ToList())
            {
                string[] SplitedString = i.SelectedServiceType.Split(':');
                foreach (var ii in SplitedString)
                {
                    var s = db.ServiceTypes.ToList().Find(x => x.ServiceTypeID.ToString() == ii);
                    if (string.IsNullOrWhiteSpace(StringForParsing))
                    {
                        StringForParsing = s.Description;
                    }
                    else
                    {
                        StringForParsing = StringForParsing + ", " + s.Description;
                    }
                }
                i.SelectedServiceType = StringForParsing;
                listOfLogs.Add(i);
                StringForParsing = "";
            }

            return View(listOfLogs);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

       
    }
}