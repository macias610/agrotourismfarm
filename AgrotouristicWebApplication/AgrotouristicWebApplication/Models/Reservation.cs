using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AgrotouristicWebApplication.Models
{
    public class Reservation
    {

        public static List<string> states = new List<string>(new string[] { "oczekiwanie", "zarezerwowano","zakończona" });

        public Reservation()
        {
            this.Reservation_House = new HashSet<Reservation_House>();
            this.Attraction_Reservation = new HashSet<Attraction_Reservation>();
        }

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

        public string ClientId { get; set; }

        public virtual ICollection<Reservation_House> Reservation_House { get; set; }
        public virtual ICollection<Attraction_Reservation> Attraction_Reservation { get; set; }
        public virtual Client Client { get; set; }
    }

}