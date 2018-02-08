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
       private DriftContext db = new DriftContext();
       private List<Log> ListOfLogs = new List<Log>();
       private string StringForParsing = "";

        // GET: Log
        public ActionResult Index(string searchString, string searchDate)
        {
            FindAndParserSelectedServiceType();

            List<Log> ListAfterSearch = new List<Log>();
            List<Log> ListForRemoveBasisDate = new List<Log>();

            if (!string.IsNullOrEmpty(searchString) || (!string.IsNullOrEmpty(searchDate)))
            {
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
                    DateTime SelectedDate = Convert.ToDateTime(searchDate).AddDays(1);
                    DateTime TwoWeeksBefore = SelectedDate.AddDays(-14);
                    foreach (var i in ListOfLogs)
                    {
                        if (i.Date > SelectedDate || i.Date < TwoWeeksBefore)
                        {
                            ListForRemoveBasisDate.Add(i);
                        }
                    }

                    foreach (var i in ListForRemoveBasisDate)
                    {
                        ListOfLogs.Remove(i);
                    }

                    ViewBag.SelectedDate = searchDate;
                }
                return View(ListOfLogs.OrderByDescending(o => o.Date).ToList());
            }

            else if(string.IsNullOrEmpty(searchDate))
            {
                ViewBag.SelectedDate = " Enter date...";
            }

            return View(ListOfLogs.OrderByDescending(o => o.Date).ToList().Take(50));
        }

        public ActionResult Details(int? id)
        {
            FindAndParserSelectedServiceType();
            var l = ListOfLogs.Find(x => x.LogID == id);
            ViewBag.SelectedServiceType = l.SelectedServiceType;
            return View(l);
        }

        public ActionResult About()
        {
            return View();
        }

            public void FindAndParserSelectedServiceType()
        {
            foreach (var i in db.Logs.ToList())
            {
                string[] SplitedString = i.SelectedServiceType.Split(':');
                foreach (var ii in SplitedString)
                {
                    var s = db.ServiceTypes.ToList().Find(x => x.ServiceTypeID.ToString() == ii);
                    if(s != null)
                    {
                        if (string.IsNullOrWhiteSpace(StringForParsing))
                        {
                            StringForParsing = s.Description;
                        }
                        else
                        {
                            StringForParsing = StringForParsing + ", " + s.Description;
                        }
                    }
                    
                }
                i.SelectedServiceType = StringForParsing;
                ListOfLogs.Add(i);
                StringForParsing = "";
            }

        }
    }
}