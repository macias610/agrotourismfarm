using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.RegularExpressions;
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
        [RegularExpression("^[1-9]{1}-(osobowy)")]
        [MinLength(6), MaxLength(15)]
        public string Type { get; set; }

        [Display(Name = "Cena:")]
        [Required]
        public decimal Price { get; set; }

        public virtual ICollection <House> House { get; set; }

    }
}