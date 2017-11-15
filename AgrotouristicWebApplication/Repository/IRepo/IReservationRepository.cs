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
        Reservation GetReservationById(int id);
        IList<Reservation> GetReservationsByState(string state);
        Reservation GetReservationBasedOnData(NewReservation reservation,string userId);
        NewReservation RetreiveExistingReservation(Reservation reservation);    
        Attraction_Reservation GetDetailsAboutReservedAttraction(int id);
        IList<Attraction> GetAttractionsForReservation(int id);
        IList<User> GetWorkersAssignedToAttraction(int id);
        IList<Reservation> RemoveOutOfDateReservations(IList<Reservation> reservations);
        Reservation_History GetReservationHistoryBasedReservation(Reservation reservation);
        Reservation_History GetReservationHistoryById(int id);
        bool checkAvaiabilityHousesBeforeConformation(NewReservation savedReservation);

        void ChangeAssignedMeals(int id, NewReservation reservation);
        void ChangeAssignedParticipants(int id,NewReservation reservation);
        void ChangeAssignedAttractions(int id, NewReservation reservation);
        void AddReservation(Reservation reservation);
        void AddReservationHistory(Reservation_History reservationHistory);
        void SaveChanges();   
        void UpdateReservation(Reservation reservation,byte[] rowVersion);
        IList<Reservation> GetClientReservations(string id);
        User GetClientById(string id);
        IList<Reservation_History> GetClientArchiveReservations(string id);
        void SaveAssignedMealsAndHouses(int id, NewReservation reservation);
        void SaveAssignedAttractions(int id, NewReservation reservation);
        void RemoveReservation(Reservation reservation);

    }
}
