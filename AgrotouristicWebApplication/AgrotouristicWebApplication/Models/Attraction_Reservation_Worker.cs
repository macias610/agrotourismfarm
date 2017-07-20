using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AgrotouristicWebApplication.Models
{
    public class Attraction_Reservation_Worker
    {
        public Attraction_Reservation_Worker()
        {

        }

        public int Id { get; set; }
        public int WorkerId { get; set; }
        public int Attraction_ReservationId { get; set; }
        public virtual Worker Worker { get; set; }
        public virtual Attraction_Reservation Attraction_Reservation { get; set; }
    }
}