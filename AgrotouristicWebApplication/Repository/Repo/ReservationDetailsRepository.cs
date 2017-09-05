using Repository.IRepo;
using Repository.Models;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Repository.Repo
{
    public class ReservationDetailsRepository: IReservationDetailsRepository
    {
        private readonly IAgrotourismContext db;

        public ReservationDetailsRepository(IAgrotourismContext db)
        {
            this.db = db;
        }

        public List<House> GetHousesForReservation(int id)
        {
            List<House> houses = (from resHouse in db.Reservation_Houses
                                  where resHouse.ReservationId.Equals(id)
                                  join house in db.Houses on resHouse.HouseId equals house.Id
                                  select house).ToList();
            return houses;

        }
        public Meal GetHouseMealForReservation(int reservationId, int houseId)
        {
            Meal meal = (from resHouse in db.Reservation_Houses
                         where resHouse.HouseId.Equals(houseId)
                         && resHouse.ReservationId.Equals(reservationId)
                         join m in db.Meals on resHouse.MealId equals m.Id
                         select m).FirstOrDefault();
            return meal;
        }
        public List<Participant> GetParticipantsHouseForReservation(int reservationId, int houseId)
        {
            List<Participant> participants = (from resHouse in db.Reservation_Houses
                                              where resHouse.HouseId.Equals(houseId)
                                              && resHouse.ReservationId.Equals(reservationId)
                                              join participant in db.Participants on resHouse.Id equals participant.Reservation_HouseId
                                              select participant).ToList();
            return participants;
        }
        public List<House> GetAvaiableHousesInTerm(DateTime startDate, DateTime endDate)
        {
            List<int> reservationHousesIDs = (from reservation in db.Reservations
                                              where (startDate.CompareTo(reservation.StartDate) >= 0 & startDate.CompareTo(reservation.EndDate) < 0)
                                              | (endDate.CompareTo(reservation.StartDate) >= 0 & endDate.CompareTo(reservation.EndDate) < 0)
                                              | (reservation.StartDate.CompareTo(startDate) >= 0 & reservation.StartDate.CompareTo(endDate) < 0)
                                              | (reservation.EndDate.CompareTo(startDate) >= 0 & reservation.EndDate.CompareTo(endDate) < 0)
                                              join resHouse in db.Reservation_Houses on reservation.Id equals resHouse.ReservationId
                                              select resHouse.HouseId).ToList();

            List<House> houses = db.Houses.Include("HouseType").ToList();
            houses.RemoveAll(item => reservationHousesIDs.Contains(item.Id));
            return houses;
        }
        public List<SelectListItem> GetNamesAvaiableHouses(List<House> houses)
        {
            List<SelectListItem> selectList = houses.Select(house => new SelectListItem { Value = house.HouseType.Type + "(" + house.Name + ")|" + "(" + house.HouseType.Price + "[zł]/doba)" + ";", Text = house.HouseType.Type + "(" + house.Name + ")" + ";" }).ToList();
            return selectList;
        }
        public House GetHouseByName(string name)
        {
            House house = (from h in db.Houses
                           where h.Name.Equals(name)
                           select h).FirstOrDefault();
            return house;
        }
        public List<SelectListItem> GetNamesAvaiableMeals()
        {
            List<Meal> avaiableMeals = db.Meals.AsNoTracking().ToList();
            List<SelectListItem> selectList = avaiableMeals.Select(avaiableMeal => new SelectListItem { Value = avaiableMeal.Type + "(" + avaiableMeal.Price + "[zł]-os./dzień" + ")", Text = avaiableMeal.Type + "(" + avaiableMeal.Price + "[zł]-os./dzień" + ")" }).ToList();
            return selectList;
        }
        public List<Participant> CopyParticipantsData(List<Participant> targetList, List<Participant> actualList)
        {
            if(targetList.First().Id ==0)
            {
                return actualList;
            }
            for (int i = 0; i < targetList.Count; i++)
            {
                targetList.ElementAt(i).Name = actualList[i].Name;
                targetList.ElementAt(i).Surname = actualList[i].Surname;
                targetList.ElementAt(i).BirthDate = actualList[i].BirthDate;
            }
            return targetList;
        }
        public List<SelectListItem> GetSelectedHousesMeals(Dictionary<string, int> dictionary, bool longVersion)
        {
            List<string> result = new List<string>();
            foreach (KeyValuePair<string, int> item in dictionary)
            {
                string mealFullName = (from meal in db.Meals
                                       where meal.Id.Equals(item.Value)
                                       select meal.Type + "(" + meal.Price + "[zł]-os./dzień)").FirstOrDefault();
                result.Add(item.Key + mealFullName);
            }
            List<SelectListItem> list = result.Select(item => new SelectListItem { Text = longVersion ? item : item.Split(';')[0] + ';', Value = longVersion ? item : item.Split(';')[0] + ';', Selected = true }).ToList();
            return list;
        }

        public void SaveSelectedHouses(NewReservation reservation, List<string> selectedHouses)
        {
            foreach (string house in selectedHouses)
            {
                int quantity = Int32.Parse(house.Split('-')[0]);
                List<Participant> participants = new List<Participant>(quantity);
                participants.AddRange(Enumerable.Repeat(new Participant(), quantity));
                reservation.AssignedParticipantsHouses.Add(house, new List<Participant>(participants));
                reservation.AssignedHousesMeals.Add(house, -1);
            }
        }
        public void SaveAssignedMealsToHouses(NewReservation reservation, List<string> selectedMeals)
        {
            foreach (string houseMeal in selectedMeals)
            {
                string houseName = houseMeal.Split(';')[0] + ';';
                string mealType = houseMeal.Split(';')[1].Split('(')[0];
                int mealId = db.Meals.Where(item => item.Type.Equals(mealType)).FirstOrDefault().Id;
                reservation.AssignedHousesMeals[houseName] = mealId;
            }
        }
        public void ClearParticipantsFormular(NewReservation reservation)
        {
            foreach (KeyValuePair<string, List<Participant>> item in reservation.AssignedParticipantsHouses)
            {
                foreach (Participant participant in item.Value)
                {
                    participant.Name = String.Empty;
                    participant.Surname = String.Empty;
                }
            }
        }
        public bool ValidateFormularParticipants(Dictionary<string, List<Participant>> dictionary)
        {
            foreach (KeyValuePair<string, List<Participant>> item in dictionary)
            {
                foreach (Participant participant in item.Value)
                {
                    var context = new ValidationContext(participant, serviceProvider: null, items: null);
                    var results = new List<ValidationResult>();
                    if (!Validator.TryValidateObject(participant, context, results, true))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}