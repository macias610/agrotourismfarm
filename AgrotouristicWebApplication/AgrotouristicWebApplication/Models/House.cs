using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AgrotouristicWebApplication.Models
{
    public class House
    {

        public House()
        {
            this.Reservation_House = new HashSet<Reservation_House>();
        }

        public ICollection<Reservation_House> Reservation_House { get; set; }
    }
}