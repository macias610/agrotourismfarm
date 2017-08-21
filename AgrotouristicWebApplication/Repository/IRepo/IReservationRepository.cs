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
        IQueryable<Reservation> GetReservations();
        Reservation GetReservationById(int id);
        List<House> GetHousesForReservation(int id);
        House GetHouseByName(string name);
        Dictionary<string,ReservationHouseDetails> ConvertToDictionaryHouseDetails(List<House> houses);
        List<SelectListItem> GetAllNamesReservedHouses(List<string> keys);
        List<SelectListItem> GetAllNamesAvaiableHouses(List<House> houses);
        List<SelectListItem> GetAllNamesAvaiableMeals();
        List<SelectListItem> AppendToListItem(List<SelectListItem> list,string item);
        List<SelectListItem> RemoveFromListItem(List<SelectListItem> list, string item);
        Meal GetHouseMealForReservation(int id);
        List<Participant> GetParticipantsHouseForReservation(int id);
        Attraction_Reservation GetDetailsAboutReservedAttraction(int id);
        List<Attraction> GetAttractionsForReservation(int id);
        List<User> GetWorkersAssignedToAttraction(int id);
        List<House> GetAvaiableHousesInTerm(DateTime startDate, DateTime endDate);
        void SaveAssignedMealsToHouses(NewReservation reservation);
        IQueryable<Reservation> GetClientReservations(string id);
    }
}
