using Repository.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Repository.Models;
using Repository.ViewModels;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;

namespace Repository.Repo
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly IAgrotourismContext db;

        public ReservationRepository(IAgrotourismContext db)
        {
            this.db = db;
        }

        private decimal CalculateCostHouses(List<string> houses)
        {
            decimal result = 0;
            foreach (string house in houses)
            {
                string houseName = Regex.Match(house, @"\(([^)]*)\)").Groups[1].Value;
                result += (from houseType in db.HouseTypes
                           join hh in db.Houses on houseType.Id equals hh.HouseTypeId
                           where hh.Name.Equals(houseName)
                           select houseType.Price).FirstOrDefault();

            }
            return result;
        }

        private decimal CalculateCostMeals(Dictionary<string,int> selectedMeals)
        {
            decimal result = 0;
            int quantity = 0;
            foreach(KeyValuePair<string,int> item in selectedMeals)
            {
                quantity = Int32.Parse(item.Key.Split('-')[0]);
                result += quantity * (db.Meals.Find(item.Value).Price);
            }
            return result;
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

        public List<User> GetWorkersAssignedToAttraction(int id)
        {
            List<User> workers = (from attrResWor in db.Attraction_Reservation_Worker
                                  where attrResWor.Attraction_ReservationId.Equals(id)
                                  join worker in db.ApplicationUsers on attrResWor.WorkerId equals worker.Id
                                  select worker).ToList();
            return workers;
        }

        public void SaveAssignedMealsToHouses(NewReservation reservation,List<string> selectedMeals)
        {
            foreach(string houseMeal in selectedMeals)
            {
                string houseName = houseMeal.Split(';')[0] + ';';
                string mealType = houseMeal.Split(';')[1].Split('(')[0];
                int mealId = db.Meals.Where(item => item.Type.Equals(mealType)).FirstOrDefault().Id;
                reservation.AssignedHousesMeals[houseName] = mealId;

            }
        }

        public void SaveSelectedHouses(NewReservation reservation,List<string> selectedHouses)
        {
            foreach(string house in selectedHouses)
            {
                int quantity = Int32.Parse(house.Split('-')[0]);
                List<Participant> participants = new List<Participant>(quantity);
                participants.AddRange(Enumerable.Repeat(new Participant(), quantity));
                reservation.AssignedParticipantsHouses.Add(house, new List<Participant>(participants));
                reservation.AssignedHousesMeals.Add(house, -1);
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

        public Reservation GetReservationBasedOnData(NewReservation reservation,string userId)
        {
            Reservation savedReservation = new Reservation()
            {
                ClientId =  userId,
                DeadlinePayment = DateTime.Now.AddDays(1),
                StartDate = reservation.StartDate,
                EndDate = reservation.EndDate,
                Status = Reservation.states[0],
                OverallCost = CalculateCostHouses(reservation.AssignedParticipantsHouses.Keys.ToList())+CalculateCostMeals(reservation.AssignedHousesMeals)
            };
            return savedReservation;
        }

        public void AddReservation(Reservation reservation)
        {
            db.Reservations.Add(reservation);
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }
    }
}