using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Repository.Models
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

        [Display(Name = "Cena:")]
        [Required]
        public decimal Price { get; set; }

        public virtual ICollection<Reservation_House> Reservation_House { get; set; }
    }
}