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
        Reservation GetReservationBasedOnData(NewReservation reservation,string userId);
        List<House> GetHousesForReservation(int id);
        House GetHouseByName(string name);
        List<SelectListItem> GetNamesAvaiableHouses(List<House> houses);
        List<SelectListItem> GetNamesAvaiableMeals();
        Meal GetHouseMealForReservation(int reservationId, int houseId);
        List<Participant> GetParticipantsHouseForReservation(int reservationId, int houseId);
        Attraction_Reservation GetDetailsAboutReservedAttraction(int id);
        List<Attraction> GetAttractionsForReservation(int id);
        List<User> GetWorkersAssignedToAttraction(int id);
        List<House> GetAvaiableHousesInTerm(DateTime startDate, DateTime endDate);
        void SaveAssignedMealsToHouses(NewReservation reservation,List<string> selectedMeals);
        void SaveSelectedHouses(NewReservation reservation,List<string> selectedHouses);
        void AddReservation(Reservation reservation);
        void SaveChanges();
        bool ValidateFormularParticipants(Dictionary<string, List<Participant>> dictionary);
        IQueryable<Reservation> GetClientReservations(string id);
        void SaveAssignedMealsAndHouses(int id, NewReservation reservation);
    }
}
