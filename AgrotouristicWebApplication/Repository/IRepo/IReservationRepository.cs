using DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ViewModel;

namespace Repository.IRepo
{
    public interface IReservationRepository
    {
        IList<Reservation> GetReservations();
        Reservation GetReservationById(int id);
        IList<Reservation> GetReservationsByState(string state);  
        Reservation_History GetReservationHistoryById(int id);
        void AddReservation(Reservation reservation);
        void AddReservationHistory(Reservation_History reservationHistory);
        void SaveChanges();   
        void UpdateReservation(Reservation reservation,byte[] rowVersion);
        User GetClientById(string id);
        IList<Reservation_History> GetClientArchiveReservations(string id);
        void RemoveReservation(Reservation reservation);

    }
}
