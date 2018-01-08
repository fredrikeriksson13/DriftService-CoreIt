using DriftService.Context;
using DriftService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DriftService.Views.Contact
{
    public class LogController : Controller
    {
        DriftContext db = new DriftContext();
        List<Log> ListOfLogs = new List<Log>();

        // GET: Log
        public ActionResult Index(string searchString, string searchDate)
        {
            ListOfLogs = db.Logs.ToList();
            List<Log> ListAfterSearch = new List<Log>();
            List<Log> ListForRemoveBasisDate = new List<Log>();

            if (!string.IsNullOrEmpty(searchString))
            {
                foreach (var i in ListOfLogs)
                {
                    if ((i.Text.ToLower().Contains(searchString.ToLower()) || i.HeadLine.ToLower().Contains(searchString.ToLower())) && (!ListAfterSearch.Any(x => x == i)))
                    {
                        ListAfterSearch.Add(i);
                    }
                }
                ListOfLogs = ListAfterSearch;
            }

            if (!string.IsNullOrEmpty(searchDate))
            {               
                foreach (var i in ListOfLogs)
                {
                    var dbDate = i.Date.ToString().Remove(10,9);
                    if (dbDate != searchDate)
                    {
                        ListForRemoveBasisDate.Add(i);
                    }
                }
                foreach (var i in ListForRemoveBasisDate)
                {
                    ListOfLogs.Remove(i);
                }
            }
            ListOfLogs = ListOfLogs.OrderByDescending(o => o.Date).ToList();
            ViewBag.SelectedDate = searchDate;

            return View(ListOfLogs);
        }

        public ActionResult Details(int? id)
        {
            var log = db.Logs.Find(id);
            return View(log);
        }
    }
}