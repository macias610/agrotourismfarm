using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Repository.ViewModels
{
    public class HousesSelection
    {
        public IEnumerable<SelectListItem> AvaiableHouses { get; set; }

        public IEnumerable<SelectListItem> SelectedHouses  { get; set; }

    }
}