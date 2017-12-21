using Repository.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DomainModel.Models;

namespace Repository.Repo
{
    public class ParticipantRepository : IParticipantRepository
    {
        private readonly IAgrotourismContext db;

        public ParticipantRepository(IAgrotourismContext db)
        {
            this.db = db;
        }

        public IList<Participant> GetParticipantsForHouseReservation(int id)
        {
            IList<Participant> participants = (from participant in db.Participants
                                               where participant.Reservation_HouseId.Equals(id)
                                               select participant).ToList();
            return participants;
        }
    }
}