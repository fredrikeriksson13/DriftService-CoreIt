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
        private DriftContext db = new DriftContext();
        private List<ContactViewModel> contactViewModelTempList = new List<ContactViewModel>();
        private List<ContactViewModel> contactsWithMatchingServiceType = new List<ContactViewModel>();

        public ActionResult Index(string sortOrder, string searchString, int[] SelectedServiceType)
        {
           

            foreach (var i in db.Contacts)
            {
                ContactViewModel contactViewModel = new ContactViewModel
                {
                    ContactID = i.ContactID,
                    FirstName = i.FirstName,
                    LastName = i.LastName,
                    Email = i.Email,
                    PhoneNumber = i.PhoneNumber,
                    Business = i.Business,
                    RegDate = i.RegDate,
                    NotificationType = i.NotificationType,
                    ContactServiceTypeList = (from c in db.ContactServiceTypes where c.ContactID == i.ContactID select c).ToList(), 
                };

                contactViewModelTempList.Add(contactViewModel);
            }
            if (SelectedServiceType != null)
            {
                foreach (var s in SelectedServiceType)
                {
                    foreach (var c in contactViewModelTempList)
                    {
                        if (db.ContactServiceTypes.Any(x => x.ContactID.Equals(c.ContactID) && x.ServiceTypeID.Equals(s)))
                        {
                            contactsWithMatchingServiceType.Add(c);
                        }
                        else if((s == 0) && !db.ContactServiceTypes.Any(x => x.ContactID.Equals(c.ContactID)))
                        {
                        
                            contactsWithMatchingServiceType.Add(c);
                        }
                    }
                }
                contactViewModelTempList = contactsWithMatchingServiceType;
            }


            if (!string.IsNullOrEmpty(searchString))
            {
               
                contactViewModelTempList = (from c in contactViewModelTempList
                                            where c.FirstName.ToLower().StartsWith(searchString.ToLower())
                                            || c.LastName.ToLower().StartsWith(searchString.ToLower())
                                            || c.Business.ToLower().StartsWith(searchString.ToLower())
                                            select c).ToList();
            }

            ViewBag.FirstNameSortParm = string.IsNullOrEmpty(sortOrder) ? "Contacts_FirstName" : "";
            ViewBag.LastNameSortParm = string.IsNullOrEmpty(sortOrder) ? "Contacts_LastName" : "";
            ViewBag.CompanyNameSortParm = string.IsNullOrEmpty(sortOrder) ? "Company_LastName" : "";
            ViewBag.RegisteredSortParm = string.IsNullOrEmpty(sortOrder) ? "Register_Date" : "";

            if (sortOrder != null)
            {
                switch (sortOrder)
                {
                    case "Contacts_FirstName":
                        contactViewModelTempList = (from x in contactViewModelTempList orderby x.FirstName select x).ToList();
                        break;
                    case "Contacts_LastName":
                        contactViewModelTempList = (from x in contactViewModelTempList orderby x.LastName select x).ToList();
                        break;
                    case "Company_LastName":
                        contactViewModelTempList = (from x in contactViewModelTempList orderby x.Business select x).ToList();
                        break;
                    case "Register_Date":
                        contactViewModelTempList = (from x in contactViewModelTempList orderby x.RegDate descending select x).ToList();
                        break;
                    default:
                        contactViewModelTempList = (from x in contactViewModelTempList orderby x.ContactID descending select x).ToList();
                        break;
                }
            }
            
            ContactViewModelList contactViewModelList = new ContactViewModelList();
            contactViewModelList.contactViewModels = contactViewModelTempList;
            contactViewModelList.ServiceTypes = db.ServiceTypes.ToList();
            if(SelectedServiceType != null)
            contactViewModelList.SelectedServiceTypeList = SelectedServiceType.ToList();
            

            if (contactViewModelList == null)
            {
                return HttpNotFound();
            }

            return View(contactViewModelList);
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
                RegDate = contact.RegDate,
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
           ContactViewModel.PhoneNumber = "+46";

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
                if (!ModelState.IsValid || (db.Contacts.Any(x => x.Email == contactViewModel.Email)) || (db.Contacts.Any(x => x.PhoneNumber == contactViewModel.PhoneNumber))  || (contactViewModel.SelectedSms == false && contactViewModel.SelectedEmail == false))//ha alla vilkor for icke-Godkänd
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
                    //if (SelectedServiceType == null)
                    //{
                    //    ViewBag.ErrorMessageServiceType = "Atlest one servicetype must be selected.";
                    //}
                    if(contactViewModel.SelectedSms == false && contactViewModel.SelectedEmail == false)
                    {
                        ViewBag.ErrorMessageNotificationType = "Atlest one Notificationtype must be selected.";
                    }
                    //If Something dosent validate, return error And selected values

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

                    int ID;
                    if (db.Contacts.Count() != 0) //Gets and sets Contacts ID
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

                      if(SelectedServiceType != null)
                      {
                        foreach (var i in SelectedServiceType) //kolla om detta kan sparas i contact obj direkt
                        {
                            ContactServiceType contactServiceType = new ContactServiceType();
                            contactServiceType.ContactID = ID;
                            contactServiceType.ServiceTypeID = i;
                            db.ContactServiceTypes.Add(contactServiceType);
                        }
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
                        RegDate = DateTime.Now,
                        ContactID = ID,
                        Language = contactViewModel.SelectedLanguage,
                    };

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
            return PartialView(contactViewModel);
        }

        // POST: Contact/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ContactViewModel contactViewModel, int[] SelectedServiceType)
        {
            try
            {
                if (!ModelState.IsValid || (db.Contacts.Any(x => x.Email == contactViewModel.Email && x.ContactID != contactViewModel.ContactID)) || (db.Contacts.Any(x => x.PhoneNumber == contactViewModel.PhoneNumber && x.ContactID != contactViewModel.ContactID)) || (contactViewModel.SelectedSms == false && contactViewModel.SelectedEmail == false))
                {
                    contactViewModel.SelectedSms = contactViewModel.SelectedSms;
                    contactViewModel.SelectedEmail = contactViewModel.SelectedEmail;
                    contactViewModel.ServiceTypeList = db.ServiceTypes.ToList();

                    if (db.Contacts.Any(x => x.Email == contactViewModel.Email && x.ContactID != contactViewModel.ContactID))
                    {
                        ModelState.AddModelError("Email", "Email already exists.");
                    }
                    if (db.Contacts.Any(x => x.PhoneNumber == contactViewModel.PhoneNumber && x.ContactID != contactViewModel.ContactID))
                    {
                        ModelState.AddModelError("PhoneNumber", "PhoneNumber already exists.");
                    }
                    //if (SelectedServiceType == null)
                    //{
                    //    ViewBag.ErrorMessageServiceType = "Atlest one servicetype must be selected.";
                    //}
                    if (contactViewModel.SelectedSms == false && contactViewModel.SelectedEmail == false)
                    {
                        ViewBag.ErrorMessageNotificationType = "Atlest one Notificationtype must be selected.";
                    }
                    //If Something dosent validate, return error And selected values

                    if (SelectedServiceType != null)
                    {
                        contactViewModel.SelectedServiceTypeList = SelectedServiceType.ToList();
                    }
                   
                    //return JavaScript()
                    //return PartialView("Edit",contactViewModel);
                    return PartialView(contactViewModel);
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
                    if (SelectedServiceType != null)
                    {
                        //Add a new Set of ContactServices
                        foreach (var i in SelectedServiceType)
                        {
                            ContactServiceType cst = new ContactServiceType { ContactID = contactViewModel.ContactID, ServiceTypeID = i };
                            db.ContactServiceTypes.Add(cst);
                        }
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
                    db.Entry(contact).Property(x => x.RegDate).IsModified = false;
                    db.Entry(contact).Property(x => x.ContactGuid).IsModified = false;
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
