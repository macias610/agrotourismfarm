using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AgrotouristicWebApplication.Models
{
    public class Attraction_Reservation
    {
        public int Id { get; set; }
        public int AttractionId { get; set; }
        public int ReservationId { get; set; }

        [Display(Name = "Termin wydarzenia:")]
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime TermAffair { get; set; }

        [Required]
        [MaxLength(2)]
        [MinLength(1)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Podać wartość liczbową")]
        public int QuantityParticipant { get; set; }

        public virtual Attraction Attraction { get; set; }
        public virtual Reservation Reservation { get; set; }
    }
}