using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DomainModel.Models
{
    public class Participant
    {

        public Participant()
        {

        }

        [Display(Name ="Id:")]
        [Required]
        public int Id { get; set; }

        [Display(Name ="Imię:")]
        [Required]
        [MinLength(3),MaxLength(15)]
        public string Name { get; set; }

        [Display(Name ="Nazwisko:")]
        [Required]
        [MinLength(3),MaxLength(15)]
        public string Surname  { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public int Reservation_HouseId { get; set; }
        public virtual ReservationHouse Reservation_House { get; set; }

    }
}