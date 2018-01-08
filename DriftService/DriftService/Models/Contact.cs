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

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Business { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int NotificationType { get; set; }

        public virtual ICollection<ContactServiceType> ContactServiceType { get; set; }
    }
}