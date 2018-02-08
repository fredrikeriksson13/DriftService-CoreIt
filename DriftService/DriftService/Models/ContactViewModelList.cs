using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DriftService.Models
{
    public class ContactViewModelList
    {
        public List<ContactViewModel> contactViewModels { get; set; }
        public List<ServiceType> ServiceTypes { get; set; }
        public List<int> SelectedServiceTypeList { get; set; }
    }
}