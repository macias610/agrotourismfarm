using DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ViewModel;

namespace Service.IService
{
    public interface IParticipantReservationService
    {
        IList<Participant> GetParticipantsHouseForReservation(int reservationId, int houseId);
        IList<Participant> CopyParticipantsData(IList<Participant> tagetList, IList<Participant> actualList);
        IList<SelectListItem> GetParticipantsQuantity(int quantity);
        Dictionary<string, List<Participant>> RetreiveHouseParticipants(int id);
        void ClearParticipantsFormular(NewReservation reservation);
        void ChangeAssignedParticipants(int id, NewReservation reservation);
        bool ValidateFormularParticipants(Dictionary<string, List<Participant>> dictionary);
    }
}
