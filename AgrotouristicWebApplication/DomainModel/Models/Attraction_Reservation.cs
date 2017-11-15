using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DomainModel.Models
{
    public class Attraction_Reservation
    {

        public Attraction_Reservation()
        {
            this.Attraction_Reservation_Worker = new HashSet<Attraction_Reservation_Worker>();
        }

        public int Id { get; set; }
        public int AttractionId { get; set; }
        public int ReservationId { get; set; }

        [Display(Name = "Termin wydarzenia:")]
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime TermAffair { get; set; }

        [Required]
        [Range(2,20)]
        [RegularExpression("^[0-9]*$")]
        public int QuantityParticipant { get; set; }
        public decimal OverallCost { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual Attraction Attraction { get; set; }
        public virtual Reservation Reservation { get; set; }
        public virtual ICollection<Attraction_Reservation_Worker> Attraction_Reservation_Worker { get; set; }
    }
}