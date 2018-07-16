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
        IList<ReservationHistory> GetClientArchiveReservations(string id);
        ReservationHistory GetReservationHistoryBasedReservation(Reservation reservation);
        ReservationHistory GetReservationHistoryById(int id);
        void AddReservationHistory(ReservationHistory reservationHistory);
    }
}
