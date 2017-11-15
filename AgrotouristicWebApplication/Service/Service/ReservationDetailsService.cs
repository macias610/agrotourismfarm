using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainModel.Models;
using System.Web;
using System.Web.Mvc;
using ViewModel;
using Repository.IRepo;

namespace Service.Service
{
    public class ReservationDetailsService : IReservationDetailsService
    {
        private readonly IReservationDetailsRepository reservationDetailsRepository = null;

        public ReservationDetailsService(IReservationDetailsRepository reservationDetailsRepository)
        {
            this.reservationDetailsRepository = reservationDetailsRepository;
        }

        public void ClearParticipantsFormular(NewReservation reservation)
        {
            this.reservationDetailsRepository.ClearParticipantsFormular(reservation);
        }

        public IList<Participant> CopyParticipantsData(IList<Participant> tagetList, IList<Participant> actualList)
        {
            IList<Participant> participants = this.reservationDetailsRepository.CopyParticipantsData(tagetList, actualList);
            return participants;
        }

        public Attraction GetAttractionByName(string name)
        {
            Attraction attraction = this.reservationDetailsRepository.GetAttractionByName(name);
            return attraction;
        }

        public Dictionary<DateTime, List<string>> GetAttractionsInGivenWeek(string term, Dictionary<DateTime, List<string>> dictionary)
        {
            Dictionary<DateTime, List<string>> attractions = this.reservationDetailsRepository.GetAttractionsInGivenWeek(term, dictionary);
            return attractions;
        }

        public IList<SelectListItem> GetAvaiableAttractions()
        {
            IList<SelectListItem> attractions = this.reservationDetailsRepository.GetAvaiableAttractions();
            return attractions;
        }

        public IList<SelectListItem> GetAvaiableDatesInWeek(string term)
        {
            IList<SelectListItem> dates = this.reservationDetailsRepository.GetAvaiableDatesInWeek(term);
            return dates;
        }

        public IList<House> GetAvaiableHousesInTerm(DateTime startDate, DateTime endDate)
        {
            IList<House> houses = this.reservationDetailsRepository.GetAvaiableHousesInTerm(startDate, endDate);
            return houses;
        }

        public House GetHouseByName(string name)
        {
            House house = this.reservationDetailsRepository.GetHouseByName(name);
            return house;
        }

        public Meal GetHouseMealForReservation(int reservationId, int houseId)
        {
            Meal meal = this.reservationDetailsRepository.GetHouseMealForReservation(reservationId, houseId);
            return meal;
        }

        public IList<House> GetHousesForReservation(int id)
        {
            IList<House> houses = this.reservationDetailsRepository.GetHousesForReservation(id);
            return houses;
        }

        public int GetMaxRowsToTableAttractions(Dictionary<DateTime, List<string>> dictionary)
        {
            int quantity = this.reservationDetailsRepository.GetMaxRowsToTableAttractions(dictionary);
            return quantity;
        }

        public IList<SelectListItem> GetNamesAvaiableHouses(IList<House> houses)
        {
            IList<SelectListItem> names = this.reservationDetailsRepository.GetNamesAvaiableHouses(houses);
            return names;
        }

        public IList<SelectListItem> GetNamesAvaiableMeals()
        {
            IList<SelectListItem> names = this.reservationDetailsRepository.GetNamesAvaiableMeals();
            return names;
        }

        public IList<Participant> GetParticipantsHouseForReservation(int reservationId, int houseId)
        {
            IList<Participant> participants = this.reservationDetailsRepository.GetParticipantsHouseForReservation(reservationId, houseId);
            return participants;
        }

        public IList<SelectListItem> GetParticipantsQuantity(int quantity)
        {
            IList<SelectListItem> participants = this.reservationDetailsRepository.GetParticipantsQuantity(quantity);
            return participants;
        }

        public Reservation GetReservationById(int id)
        {
            Reservation reservation = this.reservationDetailsRepository.GetReservationById(id);
            return reservation;
        }

        public IList<SelectListItem> GetSelectedHousesMeals(Dictionary<string, int> dictionary, bool longVersion)
        {
            IList<SelectListItem> housesMeals = this.reservationDetailsRepository.GetSelectedHousesMeals(dictionary, longVersion);
            return housesMeals;
        }

        public IList<SelectListItem> GetWeeksFromSelectedTerm(DateTime startDate, DateTime endDate)
        {
            IList<SelectListItem> weeks = this.reservationDetailsRepository.GetWeeksFromSelectedTerm(startDate, endDate);
            return weeks;
        }

        public Dictionary<DateTime, List<string>> InitializeDictionaryForAssignedAttractions(DateTime startDate, DateTime endDate)
        {
            Dictionary<DateTime, List<string>> dictionary = this.reservationDetailsRepository.InitializeDictionaryForAssignedAttractions(startDate, endDate);
            return dictionary;
        }

        public Dictionary<DateTime, List<string>> RetreiveAttractionsInGivenWeek(string term, int id)
        {
            Dictionary<DateTime, List<string>> dictionary = this.reservationDetailsRepository.RetreiveAttractionsInGivenWeek(term, id);
            return dictionary;
        }

        public Dictionary<string, List<Participant>> RetreiveHouseParticipants(int id)
        {
            Dictionary<string, List<Participant>> houseParticipants = this.reservationDetailsRepository.RetreiveHouseParticipants(id);
            return houseParticipants;
        }

        public void SaveAssignedMealsToHouses(NewReservation reservation, IList<string> selectedMeals)
        {
            this.reservationDetailsRepository.SaveAssignedMealsToHouses(reservation, selectedMeals);
        }

        public void SaveSelectedHouses(NewReservation reservation, IList<string> selectedHouses)
        {
            this.reservationDetailsRepository.SaveSelectedHouses(reservation, selectedHouses);
        }

        public bool ValidateFormularParticipants(Dictionary<string, List<Participant>> dictionary)
        {
            bool result = this.reservationDetailsRepository.ValidateFormularParticipants(dictionary);
            return result;
        }

        public void WriteDocument(string fileName, byte[] content, HttpResponseBase response)
        {
            HttpContext.Current.Response.Clear();
            response.ContentType = "application/pdf";
            response.ContentEncoding = System.Text.Encoding.UTF8;
            response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            response.CacheControl = "No-cache";
            response.BinaryWrite(content);
            response.Flush();
            response.SuppressContent = true;
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
    }
}
