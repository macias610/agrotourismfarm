using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AgrotouristicWebApplication.Models
{
    [Table("Worker")]
    public class Worker: ApplicationUser
    {
        public Worker()
        {
            this.Attraction_Reservation_Worker = new HashSet<Attraction_Reservation_Worker>();
        }

        public virtual ICollection<Attraction_Reservation_Worker> Attraction_Reservation_Worker { get; private set; }
    }
}