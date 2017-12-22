using DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel;

namespace Service.IService
{
    public interface IReservationService
    {
        Reservation GetReservationById(int id);
        IList<Reservation> GetReservationsByState(string state);
        IList<Reservation> GetClientReservations(string id);
        Reservation GetReservationBasedOnData(NewReservation reservation, string userId);
        NewReservation RetreiveExistingReservation(Reservation reservation);
        void RemoveOutOfDateReservations(string userId);
        bool checkAvaiabilityHousesBeforeConfirmation(NewReservation savedReservation); 
        void AddReservation(Reservation reservation);      
        void UpdateReservation(Reservation reservation, byte[] rowVersion);
        void SaveReservationHouses(int id, NewReservation reservation);       
        void RemoveReservation(Reservation reservation);
        void SendEmailAwaitingReservation(Reservation reservation);
        void SendEmailConfirmingReservation(Reservation reservation);
    }
}
