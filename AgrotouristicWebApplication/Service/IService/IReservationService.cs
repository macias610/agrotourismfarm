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
        IList<Reservation_History> GetClientArchiveReservations(string id);
        Reservation GetReservationBasedOnData(NewReservation reservation, string userId);
        NewReservation RetreiveExistingReservation(Reservation reservation);
        Attraction_Reservation GetDetailsAboutReservedAttraction(int id);
        IList<Attraction> GetAttractionsForReservation(int id);
        IList<User> GetWorkersAssignedToAttraction(int id);
        IList<Reservation> RemoveOutOfDateReservations(IList<Reservation> reservations);
        Reservation_History GetReservationHistoryBasedReservation(Reservation reservation);
        Reservation_History GetReservationHistoryById(int id);
        bool checkAvaiabilityHousesBeforeConformation(NewReservation savedReservation);

        void ChangeAssignedMeals(int id, NewReservation reservation);
        void ChangeAssignedParticipants(int id, NewReservation reservation);
        void ChangeAssignedAttractions(int id, NewReservation reservation);
        void AddReservation(Reservation reservation);
        void AddReservationHistory(Reservation_History reservationHistory);
        void UpdateReservation(Reservation reservation, byte[] rowVersion);
        void SaveAssignedMealsAndHouses(int id, NewReservation reservation);
        void SaveAssignedAttractions(int id, NewReservation reservation);
        void RemoveReservation(Reservation reservation);
        void SendEmailAwaitingReservation(Reservation reservation);
        void SendEmailConfirmingReservation(Reservation reservation);
    }
}
