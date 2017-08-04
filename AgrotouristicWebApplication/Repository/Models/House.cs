using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Repository.Models
{
    public class House
    {
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

        [NotMapped]
        [Display(Name ="Aktualny status")]
        public string statusHouse { get; set; }

        public ICollection<Reservation_House> Reservation_House { get; set; }
    }
}