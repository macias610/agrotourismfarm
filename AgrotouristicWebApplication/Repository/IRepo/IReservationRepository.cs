using Repository.Models;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Repository.IRepo
{
    public interface IReservationRepository
    {
        Reservation GetReservationById(int id);
        IQueryable<Reservation> GetReservationsByState(string state);
        Reservation GetReservationBasedOnData(NewReservation reservation,string userId);
        NewReservation RetreiveExistingReservation(Reservation reservation);    
        Attraction_Reservation GetDetailsAboutReservedAttraction(int id);
        List<Attraction> GetAttractionsForReservation(int id);
        List<User> GetWorkersAssignedToAttraction(int id);
        List<Reservation> RemoveOutOfDateReservations(List<Reservation> reservations);
        Reservation_History GetReservationHistoryBasedReservation(Reservation reservation);
        Reservation_History GetReservationHistoryById(int id);

        void ChangeAssignedMeals(int id, NewReservation reservation);
        void ChangeAssignedParticipants(int id,NewReservation reservation);
        void AddReservation(Reservation reservation);
        void AddReservationHistory(Reservation_History reservationHistory);
        void SaveChanges();   
        void UpdateReservation(Reservation reservation);
        IQueryable<Reservation> GetClientReservations(string id);
        IQueryable<Reservation_History> GetClientArchiveReservations(string id);
        void SaveAssignedMealsAndHouses(int id, NewReservation reservation);
        void RemoveReservation(Reservation reservation);
        
    }
}
