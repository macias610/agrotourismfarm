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

        [Display(Name ="Nazwa")]
        [Required]
        [MinLength(3),MaxLength(15)]
        [Index(IsUnique = true)]
        public string Name { get; set; }

        [Display(Name = "Opis:")]
        [Required]
        [MinLength(3), MaxLength(100)]
        public string Description { get; set; }
        
        [NotMapped]
        [Display(Name ="Aktualny status")]
        public string statusHouse { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public int  HouseTypeId { get; set; }

        public virtual HouseType HouseType { get; set; }

        public ICollection<Reservation_House> Reservation_House { get; set; }
    }
}