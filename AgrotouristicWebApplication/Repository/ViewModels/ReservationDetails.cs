using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Repository.ViewModels
{
    public class ReservationDetails
    {
        public ReservationDetails()
        {

        }
        public Reservation Reservation { get; set; }

        public List<ReservationAttractionDetails> ReservationAttractionDetails { get; set; }

        public IEnumerable<SelectListItem> Houses { get; set; }

        public int SelectedHouseDetailsId { get; set; }

        public string SelectedHouseDetailsText { get; set; }
    }
}