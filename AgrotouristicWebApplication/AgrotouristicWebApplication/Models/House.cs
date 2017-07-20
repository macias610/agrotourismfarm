using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AgrotouristicWebApplication.Models
{
    public class House
    {
        public static List<string> houses = new List<string>(new string[] { "2-osobowy", "3-osobowy", "4-osobowy" });

        public House()
        {
            this.Reservation_House = new HashSet<Reservation_House>();
        }

        [Display(Name = "Id:")]
        [Required]
        public int Id { get; set; }

        [Display(Name = "Opis:")]
        [Required]
        [MinLength(3), MaxLength(15)]
        public string Description { get; set; }
        [Display(Name = "Rodzaj:")]
        [Required]
        [MinLength(3), MaxLength(15)]
        public string Type { get; set; }

        [Display(Name ="Cena:")]
        [Required]
        public decimal Price { get; set; }


        public ICollection<Reservation_House> Reservation_House { get; set; }
    }
}