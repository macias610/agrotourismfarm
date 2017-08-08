using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Repository.Models
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
        [MinLength(3), MaxLength(30)]
        public string Name { get; set; }

        [Display(Name = "Opis:")]
        [Required]
        [MinLength(3), MaxLength(100)]
        public string Description { get; set; }

        [Display(Name = "Cena:")]
        [Required]
        public decimal Price { get; set; }

        [Display(Name = "Zniżka [%]:")]
        [Required]
        public decimal Discount { get; set; }

        public ICollection<Attraction_Reservation> Attraction_Reservation { get; set; }
    }
}