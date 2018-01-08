﻿using DriftService.Context;
using DriftService.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Mail;
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
                        //var subject = "<p>Subject:</p><p>{0}</p>";
                        //var body = "<p>Message:</p><p>{0}</p>";
                        var message = new MailMessage();

                        foreach (var i in ListOfContactsForMail)
                        {
                            message.Bcc.Add(new MailAddress(i.Email));
                        }

                        //message.Subject = string.Format(subject, model.Subject);
                        message.Subject = model.Subject;
                        message.Body = model.Message;
                        message.IsBodyHtml = true;

                        using (var smtp = new SmtpClient())
                        {
                            //await smtp.SendMailAsync(message); //avmarkerat enbart för testning
                        }
                    }
                    if (model.SendSms && (ListOfContactsForMail.Count != 0))
                    {
                        //ToDO: sätt in sms funktion här
                    }
                    
                    SaveMessageToLogg(model);
                    ViewBag.ConfirmationMessage = "Your message have been send";
                    ModelState.Clear();
                    return View(messageViewModel);
                }
                ViewBag.NoSubscribersMessage = "There are no contacts suscribed for your selected notifacation profile";
                
            }
            //if not valied 
            if (SelectedServiceType == null)
            {
                ViewBag.NoServiceTypSelected = "Most select a servicetype";  
            }
            if (model.SendMail == false && model.SendSms == false)
            {
                ViewBag.NoNotificationTypeSelected = "Most select a notificationtype"; 
            }

            messageViewModel.SelectedServiceType = SelectedServiceType;
            return View(messageViewModel);
        }
        
        public void SaveMessageToLogg(MessageViewModel model)
        {
            Log log = new Log()
            {
                Date = DateTime.Now,
                HeadLine = model.Subject,
                Text = model.Message
            };
            db.Logs.Add(log);
            db.SaveChanges();
        }
    }
}
