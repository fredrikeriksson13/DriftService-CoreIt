using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DriftService.Models
{
    public class ContactServiceType
    {

        [Key, Column(Order = 0)]
        public int ContactID { get; set; }
        [Key, Column(Order = 1)]
        public int ServiceTypeID { get; set; }
       
        public virtual Contact Contact { get; set; }
       
        public virtual ServiceType ServiceType { get; set; }

    }
}