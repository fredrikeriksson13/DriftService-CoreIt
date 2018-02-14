using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DriftService.Models
{
    public class ContactViewModel
    {
        public int ContactID { get; set; }
        [StringLength(20, ErrorMessage = "Name cannot be longer than 20 characters.")]
        [Required(ErrorMessage = "Please enter Firtst name.")]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [StringLength(20, ErrorMessage = "Name cannot be longer than 20 characters.")]
        [Required(ErrorMessage = "Please enter Last name.")]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [StringLength(20, ErrorMessage = "Name cannot be longer than 20 characters.")]
        [Required(ErrorMessage = "Please enter company name.")]
        [Display(Name = "Company")]
        public string Business { get; set; }

        [StringLength(30, ErrorMessage = "Email cannot be longer than 30 characters.")]
        [Required(ErrorMessage = "Please enter Email.")]
        [EmailAddress(ErrorMessage = "Invalid email Address")]
        public string Email { get; set; }

        [StringLength(20, ErrorMessage = "Phone number cannot be longer than 20 characters.")]
        [Required(ErrorMessage = "Please enter phone number.")]
        [RegularExpression(@"^\+[0-9]*$", ErrorMessage = "Phone number must start with dialling code (Ex. +46), and be numeric")]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Notification type")]
        public int NotificationType { get; set; }
        [Display(Name = "Registered")]
        public DateTime RegDate { get; set; }
        public int Language { get; set; }
        public List<ServiceType> ServiceTypeList { get; set; }

        public List<ContactServiceType> ContactServiceTypeList { get; set; }

        //Validation propertys, returns contacts selected value if the value dont pass the validation
        public bool SelectedSms { get; set; }
        public bool SelectedEmail { get; set; }
        public List<int> SelectedServiceTypeList { get; set; }

        public int SelectedLanguage { get; set; }

    }
}