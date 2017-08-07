using Repository.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Repository.ViewModels
{
    public class HouseDetails
    {
        public HouseDetails()
        {

        }
        public House House { get; set; }
        [Display(Name ="Rodzaj:")]
        public string Type { get; set; }
        [Display(Name ="Cena:")]
        public decimal Price { get; set; }

        public IEnumerable<SelectListItem> Types { get; set; }

        public int SelectedTypeId { get; set; }

        [Display(Name = "Typ domku")]
        public string SelectedTypeText { get; set; }
    }
}