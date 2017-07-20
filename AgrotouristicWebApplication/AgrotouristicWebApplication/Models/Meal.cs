using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AgrotouristicWebApplication.Models
{
    public class Meal
    {
        public Meal()
        {
            this.Reservation_House = new HashSet<Reservation_House>();
        }

        [Display(Name = "Id:")]
        [Required]
        public int Id { get; set; }

        [Display(Name = "Typ:")]
        [Required]
        public string Type { get; set; }

        public virtual ICollection<Reservation_House> Reservation_House { get; private set; }
    }
}