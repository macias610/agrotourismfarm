using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Repository.Models
{
    public class HouseType
    {
        public HouseType()
        {

        }

        [Display(Name = "Id:")]
        [Required]
        public int Id { get; set; }

        [Display(Name = "Rodzaj:")]
        [Index(IsUnique = true)]
        [Required]
        [MinLength(3), MaxLength(15)]
        public string Type { get; set; }

        [Display(Name = "Cena:")]
        [Required]
        public decimal Price { get; set; }

        public int HouseId { get; set; }

        public virtual House House { get; set; }
    }
}