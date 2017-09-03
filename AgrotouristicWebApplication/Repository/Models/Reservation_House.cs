using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repository.Models
{
    public class Reservation_House
    {
        public Reservation_House()
        {
            this.Participant = new HashSet<Participant>();
        }

        public int Id { get; set; }
        public int HouseId { get; set; }
        public virtual House House { get; set; }
        public int ReservationId { get; set; }
        public virtual Reservation Reservation { get; set; }
        public int MealId { get; set; }
        public virtual Meal Meal { get; set; }

        public virtual ICollection<Participant> Participant { get; set; }
    }
}