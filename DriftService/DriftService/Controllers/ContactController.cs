using DriftService.Context;
using DriftService.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DriftService.Controllers
{
    public class ContactController : Controller
    {
        // GET: Contact
        DriftContext db = new DriftContext();

        public ActionResult Index(string sortOrder, string searchString)
        {
            ViewBag.FirstNameSortParm = string.IsNullOrEmpty(sortOrder) ? "Contacts_FirstName" : "";
            ViewBag.LastNameSortParm = string.IsNullOrEmpty(sortOrder) ? "Contacts_LastName" : "";
            ViewBag.CompanyNameSortParm = string.IsNullOrEmpty(sortOrder) ? "Company_LastName" : "";
          
            var Contacts = (from s in db.Contacts
                            select s);

            if (Contacts == null)
            {
                return HttpNotFound();
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                Contacts = Contacts.Where(s => s.FirstName.Contains(searchString)
                                       || s.LastName.Contains(searchString) || s.Business.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "Contacts_FirstName":
                    Contacts = Contacts.OrderBy(s => s.FirstName);
                    break;
                case "Contacts_LastName":
                    Contacts = Contacts.OrderBy(s => s.LastName);
                    break;
                case "Company_LastName":
                    Contacts = Contacts.OrderBy(s => s.Business);
                    break;
                default:
                    Contacts = Contacts.OrderByDescending(s => s.ContactID);
                    break;
            }

            return View(Contacts.ToList());
        }

        // GET: Contact/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = db.Contacts.Find(id);
            
            if (contact == null)
            {
                return HttpNotFound();
            }

            var MatchingContactServiceTypeList = (from c in db.ContactServiceTypes
                                                  where c.ContactID == contact.ContactID
                                                  select c).ToList();

            ContactViewModel contactViewModel = new ContactViewModel
            {
                ContactID = contact.ContactID,
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                Business = contact.Business,
                Email = contact.Email,
                NotificationType = contact.NotificationType,
                PhoneNumber = contact.PhoneNumber,
                ServiceTypeList = db.ServiceTypes.ToList(),
                ContactServiceTypeList = MatchingContactServiceTypeList
            };
            
            return View(contactViewModel);
        }

        // GET: Contact/Create
        public ActionResult Create()
        {
           ContactViewModel ContactViewModel = new ContactViewModel();
           ContactViewModel.ServiceTypeList = db.ServiceTypes.ToList();

            if(ContactViewModel == null)
            {
                return HttpNotFound();
            }

             return View(ContactViewModel);
        }

        //POST: Contact/Create
       [HttpPost]
        public ActionResult Create(ContactViewModel contactViewModel, int[] SelectedServiceType)
        {
            try
            {
                if (!ModelState.IsValid || (db.Contacts.Any(x => x.Email == contactViewModel.Email)) || (db.Contacts.Any(x => x.PhoneNumber == contactViewModel.PhoneNumber)) || (SelectedServiceType == null) || (contactViewModel.SelectedSms == false && contactViewModel.SelectedEmail == false))//ha alla vilkor for icke-Godkänd
                {
                    contactViewModel.ServiceTypeList = db.ServiceTypes.ToList();
                    contactViewModel.SelectedSms = contactViewModel.SelectedSms;
                    contactViewModel.SelectedEmail = contactViewModel.SelectedEmail;
               
                    if(db.Contacts.Any(x => x.Email == contactViewModel.Email))
                    {
                        ModelState.AddModelError("Email","Email already exists.");
                    }
                    if (db.Contacts.Any(x => x.PhoneNumber == contactViewModel.PhoneNumber))
                    {
                        ModelState.AddModelError("PhoneNumber", "PhoneNumber already exists.");
                    }
                    if (SelectedServiceType == null)
                    {
                        ViewBag.ErrorMessageServiceType = "Atlest one servicetype must be selected.";
                    }
                    if(contactViewModel.SelectedSms == false && contactViewModel.SelectedEmail == false)
                    {
                        ViewBag.ErrorMessageNotificationType = "Atlest one Notificationtype must be selected.";
                    }
                    //If Something dosent validate, return error And selected values

                    if(SelectedServiceType != null)
                    {
                        contactViewModel.SelectedServiceTypeList = SelectedServiceType.ToList();
                    }

                    return View(contactViewModel);
                }

                if (ModelState.IsValid)
                {
                    if (contactViewModel.SelectedEmail == true)
                    {
                        contactViewModel.NotificationType = 1;
                    }
                    if (contactViewModel.SelectedSms == true)
                    {
                        contactViewModel.NotificationType = 2;
                    }
                    if (contactViewModel.SelectedSms == true && contactViewModel.SelectedEmail == true)
                    {
                        contactViewModel.NotificationType = 3;
                    }

                    Contact contact = new Contact
                    {
                        FirstName = contactViewModel.FirstName,
                        LastName = contactViewModel.LastName,
                        Business = contactViewModel.Business,
                        Email = contactViewModel.Email,
                        PhoneNumber = contactViewModel.PhoneNumber,
                        NotificationType = contactViewModel.NotificationType,
                        ContactGuid = Guid.NewGuid(),
                    };

                    int ID;
                    if(db.Contacts.Count() != 0) //Gets and sets Contacts ID
                    {
                        ID = (from c in db.Contacts
                                  select c.ContactID).Max() + 1;
                    }
                    else // Resets Contact identity 
                    {
                        db.Database.ExecuteSqlCommand("DBCC CHECKIDENT('Contacts', RESEED, 0)");
                        db.SaveChanges();
                        ID = 0;
                    }

                    foreach (var i in SelectedServiceType) //kolla om detta kan sparas i contact obj direkt
                    {
                        ContactServiceType contactServiceType = new ContactServiceType();
                        contactServiceType.ContactID = ID;
                        contactServiceType.ServiceTypeID = i;
                        db.ContactServiceTypes.Add(contactServiceType);
                    }

                    db.Contacts.Add(contact);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                ModelState.AddModelError("", "Unable to save changes. Please try again, and if the problem persists, see your system administrator.");
                contactViewModel.ServiceTypeList = db.ServiceTypes.ToList();
                contactViewModel.SelectedSms = contactViewModel.SelectedSms;
                contactViewModel.SelectedEmail = contactViewModel.SelectedEmail;
                if (SelectedServiceType != null)
                {
                    contactViewModel.SelectedServiceTypeList = SelectedServiceType.ToList();
                }
            }
            return View(contactViewModel);
        }

        // GET: Contact/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Contact contact = db.Contacts.Find(id);

            var MatchingContactServiceTypeList = (from c in db.ContactServiceTypes
                                                  where c.ContactID == contact.ContactID
                                                  select c).ToList();

            ContactViewModel contactViewModel = new ContactViewModel
            {   ContactID = (int)id,
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                Business = contact.Business,
                Email = contact.Email,
                PhoneNumber = contact.PhoneNumber,
                ServiceTypeList = db.ServiceTypes.ToList(),
                ContactServiceTypeList = MatchingContactServiceTypeList };

            if (contact.NotificationType == 1)
            {
                contactViewModel.SelectedEmail = true;
            }
            if (contact.NotificationType == 2)
            {
                contactViewModel.SelectedSms = true;
            }
            if (contact.NotificationType == 3)
            {
                contactViewModel.SelectedSms = true;
                contactViewModel.SelectedEmail = true;
            }

            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contactViewModel);
        }

        // POST: Contact/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ContactViewModel contactViewModel, int[] SelectedServiceType)
        {
            try
            {
                if (SelectedServiceType == null || (contactViewModel.SelectedSms == false && contactViewModel.SelectedEmail == false))
                {
                    contactViewModel.SelectedSms = contactViewModel.SelectedSms;
                    contactViewModel.SelectedEmail = contactViewModel.SelectedEmail;
                    contactViewModel.ServiceTypeList = db.ServiceTypes.ToList();

                    if (SelectedServiceType == null)
                    {
                        ViewBag.ErrorMessageServiceType = "Atlest one servicetype must be selected.";
                    }
                    if (contactViewModel.SelectedSms == false && contactViewModel.SelectedEmail == false)
                    {
                        ViewBag.ErrorMessageNotificationType = "Atlest one Notificationtype must be selected.";
                    }
                    if (SelectedServiceType != null)
                    {
                        contactViewModel.SelectedServiceTypeList = SelectedServiceType.ToList();
                    }
                    return View(contactViewModel);
                }
                
                if (ModelState.IsValid)
                {
                    if (contactViewModel.SelectedEmail == true)
                    {
                        contactViewModel.NotificationType = 1;
                    }
                    if (contactViewModel.SelectedSms == true)
                    {
                        contactViewModel.NotificationType = 2;
                    }
                    if (contactViewModel.SelectedSms == true && contactViewModel.SelectedEmail == true)
                    {
                        contactViewModel.NotificationType = 3;
                    }

                    //Removes the selected customers ContactServices
                    db.ContactServiceTypes.RemoveRange(db.ContactServiceTypes.Where(x => x.ContactID == contactViewModel.ContactID));

                    //Add a new Set of ContactServices
                    foreach (var i in SelectedServiceType)
                    {
                      ContactServiceType cst = new ContactServiceType { ContactID = contactViewModel.ContactID, ServiceTypeID = i };
                      db.ContactServiceTypes.Add(cst);
                    }

                    Contact contact = new Contact
                    {
                        ContactID = contactViewModel.ContactID,
                        FirstName = contactViewModel.FirstName,
                        LastName = contactViewModel.LastName,
                        Business = contactViewModel.Business,
                        Email = contactViewModel.Email,
                        PhoneNumber = contactViewModel.PhoneNumber,
                        NotificationType = contactViewModel.NotificationType,
                    };

                    db.Entry(contact).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return View(contactViewModel);
        }
        
        public ActionResult DeleteContact(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Contact contact = db.Contacts.Find(id);
                db.Contacts.Remove(contact);
                db.ContactServiceTypes.RemoveRange(db.ContactServiceTypes.Where(x => x.ContactID == contact.ContactID));
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return RedirectToAction("Index");
        }
    }
}
