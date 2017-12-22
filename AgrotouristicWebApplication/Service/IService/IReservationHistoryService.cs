using DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IReservationHistoryService
    {
        IList<Reservation_History> GetClientArchiveReservations(string id);
        Reservation_History GetReservationHistoryBasedReservation(Reservation reservation);
        Reservation_History GetReservationHistoryById(int id);
        void AddReservationHistory(Reservation_History reservationHistory);
    }
}
