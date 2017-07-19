using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AgrotouristicWebApplication.Models
{
    public class Client : ApplicationUser
    {
        public Client()
        {
            this.Reservation = new HashSet<Reservation>();
        }

        public virtual ICollection<Reservation> Reservation { get; private set; }
    }
}