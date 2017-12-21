using DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepo
{
    public interface IParticipantRepository
    {
        IList<Participant> GetParticipantsForHouseReservation(int id);
    }
}
