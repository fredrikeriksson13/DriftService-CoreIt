using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DriftService.Models
{
    public class MessageViewModel
    {
        [Required(ErrorMessage = "Message must have a subject")]
        public string Subject { get; set; }
        [Required(ErrorMessage = "Message can't be empty")]
        public string Message { get; set; }

        public bool SendMail { get; set; }
        public bool SendSms { get; set; }

        public int[] SelectedServiceType;

        public List<ServiceType> ServiceTypeList;
    }


}