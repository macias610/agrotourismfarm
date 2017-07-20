using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AgrotouristicWebApplication.Models
{
    [Table("Client")]
    public class Client : ApplicationUser
    {
        public Client()
        {
            this.Reservations = new HashSet<Reservation>();
        }
        public virtual ICollection<Reservation> Reservations { get; private set; }
    }
}