using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DriftService.Models
{
    public class ServiceType
    {
        
       [Key]
        public int ServiceTypeID { get; set; }

        public string Description { get; set; }

        public bool PublicServiceType { get; set; }

        public virtual ICollection<ContactServiceType> ContactServiceType { get; set; }
    }
}