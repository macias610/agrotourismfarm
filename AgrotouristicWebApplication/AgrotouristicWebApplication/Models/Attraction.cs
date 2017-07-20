using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AgrotouristicWebApplication.Models
{
    public class Attraction
    {

        public Attraction()
        {
            this.Attraction_Reservation = new HashSet<Attraction_Reservation>();
        }

        [Display(Name = "Id:")]
        [Required]
        public int Id { get; set; }

        [Display(Name = "Atrakcja:")]
        [Required]
        [MinLength(3), MaxLength(15)]
        public string Name { get; set; }

        [Display(Name = "Opis:")]
        [Required]
        [MinLength(3), MaxLength(15)]
        public string Description { get; set; }

        public ICollection<Attraction_Reservation> Attraction_Reservation { get; set; }
    }
}