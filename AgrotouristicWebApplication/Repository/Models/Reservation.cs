using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Repository.Models
{
    public class Reservation
    {

        public static List<string> OptionStates = new List<string>(new string[] { "-", "wszystkie","oczekiwanie", "zarezerwowano","zakończona" });
        public static List<string> States = new List<string>(new string[] { "oczekiwanie","zarezerwowano","zakończona"});

        public Reservation()
        {
            this.Reservation_House = new HashSet<Reservation_House>();
            this.Attraction_Reservation = new HashSet<Attraction_Reservation>();
        }

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

        [Display(Name = "Ostateczny termin płatności:")]
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DeadlinePayment { get; set; }

        [Display(Name = "Status:")]
        [Required]
        [MinLength(3), MaxLength(15)]
        public string Status { get; set; }

        [Display(Name = "Koszt całkowity:")]
        [Required]
        public decimal OverallCost { get; set; }

        public string ClientId { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual ICollection<Reservation_House> Reservation_House { get; set; }
        public virtual ICollection<Attraction_Reservation> Attraction_Reservation { get; set; }
        public virtual User Client { get; set; }

    }

}