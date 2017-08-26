using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Repository.ViewModels
{
    public class MealsSelection
    {
        public IEnumerable<SelectListItem> AvaiableMeals { get; set; }

        public IEnumerable<SelectListItem> SelectedHouses { get; set; }

        public IEnumerable<SelectListItem> SelectedMeals { get; set; }

        public string StringSelectedMeals { get; set; }
    }
}