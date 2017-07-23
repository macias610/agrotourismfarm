using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repository.Models
{
    public class Reservation_House_Participant
    {
        public Reservation_House_Participant()
        {

        }

        public int Id { get; set; }
        public int ParticipantId { get; set; }
        public int Reservation_HouseId { get; set; }
        public virtual Participant Participant { get; set; }
        public virtual Reservation_House Reservation_House { get; set; }
    }
}