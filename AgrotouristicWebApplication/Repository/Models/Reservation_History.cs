using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Repository.Models
{
    public class Reservation_History
    {
        [Display(Name = "Id:")]
        [Required]
        public int Id { get; set; }

        [Display(Name = "Data przyjazdu:")]
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Display(Name = "Data wyjazdu:")]
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        [Display(Name = "Koszt całkowity:")]
        [Required]
        public decimal OverallCost { get; set; }

        [Display(Name ="Zarezerwowane domki")]
        [Required]
        public string ReservedHouses { get; set; }

        [Display(Name ="Zarezerwowane warsztaty")]
        [Required]
        public string ReservedAttractions { get; set; }

        public string ClientId { get; set; }

        public virtual User Client { get; set; }
    }
}