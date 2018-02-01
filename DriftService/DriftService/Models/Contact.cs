using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DriftService.Models
{
    public class Contact
    {
        [Key]
        public int ContactID { get; set; }
        public Guid ContactGuid { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Business { get; set; }
        public string Email { get; set; }
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Notification type")]
        public int NotificationType { get; set; }
        public DateTime RegDate { get; set; }

        public virtual ICollection<ContactServiceType> ContactServiceType { get; set; }
    }
}