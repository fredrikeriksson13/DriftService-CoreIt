using DriftService.Context;
using DriftService.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DriftService.Controllers
{
    public class MessageController : Controller
    { 
        private DriftContext db = new DriftContext();
        private List<Contact> ListToSend = new List<Contact>();
        private List<Contact> ListOfContactsForMail = new List<Contact>();
        private List<Contact> ListOfContactsForSMS = new List<Contact>();
        MessageViewModel messageViewModel = new MessageViewModel();

        public MessageController()
        {
            messageViewModel.ServiceTypeList = db.ServiceTypes.ToList();
        }

        [HttpGet]
        public ActionResult Index()
        {
           return View(messageViewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Index(MessageViewModel model, int[] SelectedServiceType)
        {
            try
            {
                if ((ModelState.IsValid) && (SelectedServiceType != null) && (model.SendMail == true || model.SendSms == true))
                {
                    foreach (var i in db.Contacts.ToList()) //Sort contacts after choosen servicetype
                    {
                        foreach (var x in i.ContactServiceType)
                        {
                            if (SelectedServiceType.Any(y => y == x.ServiceTypeID) && !ListToSend.Any(m => m.ContactID == i.ContactID))
                            {
                                ListToSend.Add(i);
                            }
                        }
                    }

                    if (ListToSend.Count != 0)
                    {
                        foreach (var i in ListToSend)
                        {
                            if ((i.NotificationType == 1 || i.NotificationType == 3) && model.SendMail)
                            {
                                ListOfContactsForMail.Add(i);
                            }
                            if ((i.NotificationType == 2 || i.NotificationType == 3) && model.SendSms)
                            {
                                ListOfContactsForSMS.Add(i);
                            }
                        }

                        if (model.SendMail && (ListOfContactsForMail.Count != 0))
                        {
                            await SendEmail(model);
                        }
                        if (model.SendSms && (ListOfContactsForSMS.Count != 0))
                        {
                            await SendSms(model);
                        }

                        SaveMessageToLogg(model, SelectedServiceType);
                        ViewBag.ConfirmationMessage = "Your message have been send";
                        ModelState.Clear();
                        return View(messageViewModel);
                    }
                    ViewBag.NoSubscribersMessage = "There are no contacts subscribed for your selected notifacation profile";
                }
                //if not valied 
                if (SelectedServiceType == null)
                {
                    ViewBag.NoServiceTypSelected = "Must select a service type";
                }
                if (model.SendMail == false && model.SendSms == false)
                {
                    ViewBag.NoNotificationTypeSelected = "Must select a notification type";
                }

                messageViewModel.SelectedServiceType = SelectedServiceType;
                return View(messageViewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                ViewBag.Error = "Unable to send notification. Please try again, and if the problem persists, contact your system administrator.";
                messageViewModel.SelectedServiceType = SelectedServiceType;
                return View(messageViewModel);
            }
        }

        private async Task SendEmail(MessageViewModel model)
        {
            foreach (var i in ListOfContactsForMail)
            {
                var message = new MailMessage();
                message.To.Add(i.Email);

                string HeadtextSize = "'font-size:100%';";
                string unregisterstyle = "'font-size:75%';";
                string singnatur = "<p><em>Hälsningar: CoreIT Driftservice-Notifikation<br/>Telefon:  0660-729 99<br/>Mail: info@coreit.se<em></p>";

                string queryString = System.Configuration.ConfigurationManager.AppSettings["UnregisterLink"].ToString() + i.ContactGuid.ToString();
                string unregiserLink = "<a href='" + queryString + "'>";


                string HeadLine = "<p style=" + HeadtextSize + "><b>Hej " + i.FirstName + "!"+ "</p></b>";
                string unregister = unregiserLink + "<center><p style=" + unregisterstyle + ">Klicka här för att avregistrera</p></center></a>";

                message.Subject = "Driftservice-Notifikation: " + model.Subject;
                message.Body = HeadLine + model.Message + "<br/><br/>" + singnatur + unregister;
            
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    await smtp.SendMailAsync(message);
                }
            }
        }

        public async Task SendSms(MessageViewModel model)
        {
            try
            {
                foreach (var i in ListOfContactsForSMS)
                {
                    var client = new HttpClient();
                    client.BaseAddress = new Uri("https://api.46elks.com");

                    var user = System.Configuration.ConfigurationManager.AppSettings["ApiUser"].ToString();
                    var pwd = System.Configuration.ConfigurationManager.AppSettings["ApiPassword"].ToString();

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(string.Format("{0}:{1}", user, pwd))));

                    var content = new FormUrlEncodedContent(new[] {
                    new KeyValuePair < string, string > ("from", "Drift"), //Syns som avsändare måste vara kortare än 11chars
                    new KeyValuePair < string, string > ("to", i.PhoneNumber), //mottagare sätta +46 som fast?
                    new KeyValuePair < string, string > ("message", model.Subject + ": " + model.Message + " Mvh Driftservice"),
                    });

                    HttpResponseMessage response = await client.PostAsync("/a1/SMS", content);
                    //response.EnsureSuccessStatusCode();
                    var result = await response.Content.ReadAsStringAsync();
                }  
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void SaveMessageToLogg(MessageViewModel model, int[] selectedServiceType)
        {
            string s = "";

            foreach (int i in selectedServiceType)
            {
                if (string.IsNullOrWhiteSpace(s))
                {
                    s = i.ToString();
                }
                else
                {
                    s = s + ":" + i.ToString();
                }
            }

            Log log = new Log()
            {
                Date = DateTime.Now,
                HeadLine = model.Subject,
                Text = model.Message,
                SelectedServiceType = s
            };

            db.Logs.Add(log);
            db.SaveChanges();
        }


    }
}
