using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repository.ViewModels
{
    public class ReservationHouseDetails
    {

        public House House { get; set; }

        public List<Participant> Participants { get; set; }

        public Meal Meal { get; set; }
    }
}