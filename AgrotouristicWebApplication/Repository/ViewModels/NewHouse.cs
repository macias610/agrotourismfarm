using Repository.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Repository.ViewModels
{
    public class NewHouse
    {
        public House House { get; set; }

        public IEnumerable<SelectListItem> Types { get; set; }

        public int SelectedTypeId { get; set; }

        [Display(Name = "Typ domku")]
        public string SelectedTypeText { get; set; }
    }
}