using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repository.ViewModels
{
    public class ReservationAttractionDetails
    {
        public Attraction Attraction { get; set; }

        public Attraction_Reservation Attraction_Reservation { get; set; }

        public List<User> Workers { get; set; }
    }
}