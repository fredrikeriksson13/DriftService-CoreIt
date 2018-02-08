using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DriftService.Models
{
    public class Log
    {
        public int LogID { get; set; }
        [Display(Name = "Headline")]
        public string HeadLine { get; set; }
        public string Text { get; set; }
        public bool Webb { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        public string SelectedServiceType { get; set; }
    }
}