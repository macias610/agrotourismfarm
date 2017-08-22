using Repository.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Repository.Models;
using Repository.ViewModels;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace Repository.Repo
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly IAgrotourismContext db;

        public ReservationRepository(IAgrotourismContext db)
        {
            this.db = db;
        }

        public List<SelectListItem> AppendToListItem(List<SelectListItem> list, string item)
        {
            List<string> result = new List<string>();
            list.ForEach(elem => result.Add(elem.Value));
            result.Add(item);
            string tempStr = null;
            result = result.OrderBy(o => (tempStr = o.Split(';')[0].Split('(')[1]).Substring(0, tempStr.Length - 1)).ToList();
            List<SelectListItem> selectList = result.Select(elem => new SelectListItem { Value = elem.ToString(), Text = elem.ToString(),Selected = elem.ToString().Equals(item)?true:false }).ToList();
            return selectList;
        }

        public Dictionary<string, ReservationHouseDetails> ConvertToDictionaryHouseDetails(List<House> houses)
        {
            Dictionary<string, ReservationHouseDetails> reservationHouseDetails = new Dictionary<string, ReservationHouseDetails>();

            houses.ForEach(item => reservationHouseDetails.Add(item.Name,new ReservationHouseDetails()
            {
                House = item,
                Participants = GetParticipantsHouseForReservation(item.Id),
                Meal = GetHouseMealForReservation(item.Id)
            }
            ));
            return reservationHouseDetails;
        }

        public List<SelectListItem> GetAllNamesAvaiableHouses(List<House> houses)
        {
            List<SelectListItem> selectList = houses.Select(house => new SelectListItem { Value = house.HouseType.Type+"("+house.Name+");", Text = house.HouseType.Type + "(" + house.Name +");" }).ToList();
            return selectList;
        }

        public List<SelectListItem> GetAllNamesAvaiableMeals()
        {
            List<Meal> avaiableMeals = db.Meals.AsNoTracking().ToList();
            List<SelectListItem> selectList = avaiableMeals.Select(avaiableMeal => new SelectListItem { Value = avaiableMeal.Type+"("+avaiableMeal.Price+")", Text = avaiableMeal.Type + "(" + avaiableMeal.Price +")" }).ToList();
            return selectList;
        }

        public List<SelectListItem> GetAllNamesReservedHouses(List<string> keys)
        {
            List<SelectListItem> selectList = keys.Select(key => new SelectListItem { Value = key, Text = key }).ToList();
            return selectList;
        }

        public List<Attraction> GetAttractionsForReservation(int id)
        {
            List<Attraction> attractions = (from attrRes in db.Attraction_Reservation
                                            where attrRes.ReservationId.Equals(id)
                                            join attraction in db.Attractions on attrRes.AttractionId equals attraction.Id
                                            select attraction).ToList();
            return attractions;
        }

        public List<House> GetAvaiableHousesInTerm(DateTime startDate, DateTime endDate)
        {
            List<int> reservationHousesIDs = (from reservation in db.Reservations
                                              where (startDate.CompareTo(reservation.StartDate)>=0 & startDate.CompareTo(reservation.EndDate)<0)
                                              | (endDate.CompareTo(reservation.StartDate)>=0 & endDate.CompareTo(reservation.EndDate) <0)
                                              | (reservation.StartDate.CompareTo(startDate)>=0 & reservation.StartDate.CompareTo(endDate) <0)
                                              | (reservation.EndDate.CompareTo(startDate) >=0 & reservation.EndDate.CompareTo(endDate) <0)
                                              join resHouse in db.Reservation_House on reservation.Id equals resHouse.ReservationId
                                              select resHouse.HouseId).ToList();

            List<House> houses = db.Houses.Include("HouseType").ToList();
            houses.RemoveAll(item=>reservationHousesIDs.Contains(item.Id));
            return houses;
        }

        public IQueryable<Reservation> GetClientReservations(string id)
        {
            IQueryable<Reservation> reservations = from reservation in db.Reservations
                                                   where reservation.ClientId.Equals(id)
                                                   select reservation;
            return reservations;
        }

        public Attraction_Reservation GetDetailsAboutReservedAttraction(int id)
        {
            Attraction_Reservation attractionReservation = (from attrRes in db.Attraction_Reservation
                                                            where attrRes.AttractionId.Equals(id)
                                                            select attrRes).FirstOrDefault();
            return attractionReservation;
        }

        public House GetHouseByName(string name)
        {
            House house = (from h in db.Houses
                           where h.Name.Equals(name)
                           select h).FirstOrDefault();
            return house;
        }

        public Meal GetHouseMealForReservation(int id)
        {
            Meal meal = (from resHouse in db.Reservation_House
                         where resHouse.HouseId.Equals(id)
                         join m in db.Meals on resHouse.MealId equals m.Id
                         select m).FirstOrDefault();
            return meal;
        }

        public List<House> GetHousesForReservation(int id)
        {
            List<House> houses = (from resHouse in db.Reservation_House
                                  where resHouse.ReservationId.Equals(id)
                                  join house in db.Houses on resHouse.HouseId equals house.Id
                                  select house).ToList();
            return houses;

        }

        public List<Participant> GetParticipantsHouseForReservation(int id)
        {
            int resHouseId = (from resHouse in db.Reservation_House
                                where resHouse.HouseId.Equals(id)
                                select resHouse.Id).FirstOrDefault();

            List<Participant> participants = (from resHousePart in db.Reservation_House_Participant
                                              where resHousePart.Reservation_HouseId.Equals(resHouseId)
                                              join participant in db.Participants on resHousePart.ParticipantId equals participant.Id
                                              select participant).ToList();
            return participants;
        }

        public Reservation GetReservationById(int id)
        {
            Reservation reservation = db.Reservations.Find(id);
            return reservation;
        }

        public IQueryable<Reservation> GetReservations()
        {
            IQueryable<Reservation> reservations = db.Reservations.AsNoTracking();
            return reservations;
        }

        public List<User> GetWorkersAssignedToAttraction(int id)
        {
            List<User> workers = (from attrResWor in db.Attraction_Reservation_Worker
                                  where attrResWor.Attraction_ReservationId.Equals(id)
                                  join worker in db.ApplicationUsers on attrResWor.WorkerId equals worker.Id
                                  select worker).ToList();
            return workers;
        }

        public List<SelectListItem> RemoveFromListItem(List<SelectListItem> list, string item)
        {
            List<string> result = new List<string>();
            list.ForEach(elem => result.Add(elem.Value));
            result.Remove(item);
            List<SelectListItem> selectList = result.Select(elem => new SelectListItem { Value = elem.ToString(), Text = elem.ToString(), Selected = elem.Equals(item) ? true : false }).ToList();
            return selectList;
        }

        public void SaveAssignedMealsToHouses(NewReservation reservation)
        {
            foreach(SelectListItem houseMeal in reservation.SelectedHousesMeals.ToList())
            {
                string houseName = houseMeal.Value.Split(';')[0].TrimEnd(')').Split('(')[1];
                string mealType = houseMeal.Value.Split(';')[1].Split('(')[0];
                int houseId = db.Houses.Where(item => item.Name.Equals(houseName)).FirstOrDefault().Id;
                int mealId = db.Meals.Where(item => item.Type.Equals(mealType)).FirstOrDefault().Id;
                reservation.AssignedHousesMeals.Add(houseId, mealId);
            }
        }

        public void SaveSelectedHouses(NewReservation reservation)
        {
            foreach(SelectListItem house in reservation.SelectedHouses.ToList())
            {
                int quantity = Int32.Parse(house.Value.Split('-')[0]);
                string houseName = Regex.Match(house.Value, @"\(([^)]*)\)").Groups[1].Value;
                List<Participant> participants = new List<Participant>(quantity);
                participants.AddRange(Enumerable.Repeat(new Participant(),quantity));
                reservation.AssignedParticipantsHouses.Add(houseName, new List<Participant>(participants));
            }
        }
    }
}