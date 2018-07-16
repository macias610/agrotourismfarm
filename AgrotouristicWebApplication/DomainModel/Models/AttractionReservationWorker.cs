using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DomainModel.Models
{
    public class AttractionReservationWorker
    {
        public AttractionReservationWorker()
        {

        }

        public int Id { get; set; }
        public string WorkerId { get; set; }
        public int Attraction_ReservationId { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual User Worker { get; set; }
        public virtual AttractionReservation Attraction_Reservation { get; set; }
    }
}