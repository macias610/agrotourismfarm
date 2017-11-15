using DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ViewModel
{
    public class ReservationHouseDetails
    {
        public House House { get; set; }

        public IList<Participant> Participants { get; set; }

        public Meal Meal { get; set; }
    }
}