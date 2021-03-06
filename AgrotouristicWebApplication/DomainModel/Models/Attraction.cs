﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DomainModel.Models
{
    public class Attraction
    {

        public Attraction()
        {
            this.Attraction_Reservation = new HashSet<AttractionReservation>();
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

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public ICollection<AttractionReservation> Attraction_Reservation { get; set; }
    }
}