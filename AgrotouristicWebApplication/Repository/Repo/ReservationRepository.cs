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
using System.Data.Entity;

namespace Repository.Repo
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly IAgrotourismContext db;

        public ReservationRepository(IAgrotourismContext db)
        {
            this.db = db;
        }

        private void SaveAssignedHouseParticipants(int reservationHouseId, List<Participant> participants)
        {
            foreach (Participant participant in participants)
            {
                db.Participants.Add(participant);
                SaveChanges();
                Reservation_House_Participant resHousePart = new Reservation_House_Participant()
                {
                    ParticipantId = participant.Id,
                    Reservation_HouseId = reservationHouseId
                };
                db.Reservation_House_Participant.Add(resHousePart);
                SaveChanges();
            }
        }

        public List<SelectListItem> GetNamesAvaiableHouses(List<House> houses)
        {
            List<SelectListItem> selectList = houses.Select(house => new SelectListItem { Value = house.HouseType.Type + "(" + house.Name + ")|" + "(" + house.HouseType.Price + "[zł]/doba)" + ";", Text = house.HouseType.Type + "(" + house.Name + ")" + ";" }).ToList();
            return selectList;
        }

        public List<SelectListItem> GetNamesAvaiableMeals()
        {
            List<Meal> avaiableMeals = db.Meals.AsNoTracking().ToList();
            List<SelectListItem> selectList = avaiableMeals.Select(avaiableMeal => new SelectListItem { Value = avaiableMeal.Type + "(" + avaiableMeal.Price + "[zł]-os./dzień" + ")", Text = avaiableMeal.Type + "(" + avaiableMeal.Price + "[zł]-os./dzień" + ")" }).ToList();
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
                                              where (startDate.CompareTo(reservation.StartDate) >= 0 & startDate.CompareTo(reservation.EndDate) < 0)
                                              | (endDate.CompareTo(reservation.StartDate) >= 0 & endDate.CompareTo(reservation.EndDate) < 0)
                                              | (reservation.StartDate.CompareTo(startDate) >= 0 & reservation.StartDate.CompareTo(endDate) < 0)
                                              | (reservation.EndDate.CompareTo(startDate) >= 0 & reservation.EndDate.CompareTo(endDate) < 0)
                                              join resHouse in db.Reservation_House on reservation.Id equals resHouse.ReservationId
                                              select resHouse.HouseId).ToList();

            List<House> houses = db.Houses.Include("HouseType").ToList();
            houses.RemoveAll(item => reservationHousesIDs.Contains(item.Id));
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

        public Meal GetHouseMealForReservation(int reservationId, int houseId)
        {
            Meal meal = (from resHouse in db.Reservation_House
                         where resHouse.HouseId.Equals(houseId)
                         && resHouse.ReservationId.Equals(reservationId)
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

        public List<Participant> GetParticipantsHouseForReservation(int reservationId, int houseId)
        {
            int resHouseId = (from resHouse in db.Reservation_House
                              where resHouse.HouseId.Equals(houseId)
                              && resHouse.ReservationId.Equals(reservationId)
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

        public Reservation GetReservationBasedOnData(NewReservation reservation, string userId)
        {
            Reservation savedReservation = new Reservation()
            {
                ClientId = userId,
                DeadlinePayment = DateTime.Now.AddDays(1),
                StartDate = reservation.StartDate,
                EndDate = reservation.EndDate,
                Status = Reservation.states[0],
                OverallCost = reservation.OverallCost
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

        public void SaveAssignedMealsAndHouses(int id, NewReservation reservation)
        {
            foreach (KeyValuePair<string, int> item in reservation.AssignedHousesMeals)
            {
                string houseName = Regex.Match(item.Key, @"\(([^)]*)\)").Groups[1].Value;
                int houseId = db.Houses.Where(house => house.Name.Equals(houseName)).FirstOrDefault().Id;
                Reservation_House reservationHouse = new Reservation_House()
                {
                    HouseId = houseId,
                    MealId = item.Value,
                    ReservationId = id
                };
                db.Reservation_House.Add(reservationHouse);
                db.SaveChanges();
                SaveAssignedHouseParticipants(reservationHouse.Id, reservation.AssignedParticipantsHouses[item.Key]);

            }
        }



        public NewReservation RetreiveExistingReservation(Reservation reservation)
        {
            NewReservation exisitingReservation = new NewReservation();
            exisitingReservation.StartDate = reservation.StartDate;
            exisitingReservation.EndDate = reservation.EndDate;
            exisitingReservation.OverallCost = reservation.OverallCost;
            List<string> selectedHousesMeals = (from resHou in db.Reservation_House
                                                where resHou.ReservationId.Equals(reservation.Id)
                                                join house in db.Houses on resHou.HouseId equals house.Id
                                                join houseType in db.HouseTypes on house.HouseTypeId equals houseType.Id
                                                select (houseType.Type + "(" + house.Name + ");" + resHou.MealId)).ToList();
            foreach (string selectHouseMeal in selectedHousesMeals)
            {
                exisitingReservation.AssignedHousesMeals.Add(selectHouseMeal.Split(';')[0] + ';', Int32.Parse(selectHouseMeal.Split(';')[1]));
            }
            List<string> selectedHousesIdResHouses = (from resHou in db.Reservation_House
                                                      where resHou.ReservationId.Equals(reservation.Id)
                                                      join house in db.Houses on resHou.HouseId equals house.Id
                                                      join houseType in db.HouseTypes on house.HouseTypeId equals houseType.Id
                                                      select (houseType.Type + "(" + house.Name + ");" + resHou.Id)).ToList();
            foreach (string selectHouseIdResHou in selectedHousesIdResHouses)
            {
                int resHouId = Int32.Parse(selectHouseIdResHou.Split(';')[1]);
                List<Participant> participants = (from resHouPart in db.Reservation_House_Participant
                                                  where resHouPart.Reservation_HouseId.Equals(resHouId)
                                                  join participant in db.Participants on resHouPart.ParticipantId equals participant.Id
                                                  select participant).ToList();
                exisitingReservation.AssignedParticipantsHouses.Add(selectHouseIdResHou.Split(';')[0] + ';', participants);
            }
            return exisitingReservation;

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

        public void ChangeAssignedMeals(int id, NewReservation reservation)
        {
            foreach (KeyValuePair<string, int> item in reservation.AssignedHousesMeals)
            {
                string houseName = Regex.Match(item.Key, @"\(([^)]*)\)").Groups[1].Value;
                int houseId = db.Houses.Where(house => house.Name.Equals(houseName)).FirstOrDefault().Id;
                Reservation_House reservationHouse = (from resHouse in db.Reservation_House
                                                      where resHouse.ReservationId.Equals(id)
                                                      && resHouse.HouseId.Equals(houseId)
                                                      select resHouse).FirstOrDefault();
                reservationHouse.MealId = item.Value;
                db.Entry(reservationHouse).State = EntityState.Modified;
                Reservation editedReservation = GetReservationById(id);
                editedReservation.OverallCost = reservation.OverallCost;
                db.Entry(editedReservation).State = EntityState.Modified;
                SaveChanges();
            }
        }

        public void ChangeAssignedParticipants(int id,NewReservation reservation)
        {
            foreach (KeyValuePair<string, List<Participant>> item in reservation.AssignedParticipantsHouses)
            {
                string houseName = Regex.Match(item.Key, @"\(([^)]*)\)").Groups[1].Value;
                int houseId = db.Houses.Where(house => house.Name.Equals(houseName)).FirstOrDefault().Id;
                int reservationHouseId = (from resHouse in db.Reservation_House
                                                      where resHouse.ReservationId.Equals(id)
                                                      && resHouse.HouseId.Equals(houseId)
                                                      select resHouse.Id).FirstOrDefault();
                List<int> participantsId = (from resHouPart in db.Reservation_House_Participant
                                            where resHouPart.Reservation_HouseId.Equals(reservationHouseId)
                                            select resHouPart.ParticipantId).ToList();

                foreach(int participantId in participantsId)
                {
                    Participant participant = db.Participants.FirstOrDefault(x => x.Id.Equals(participantId));
                    Participant edited = reservation.AssignedParticipantsHouses[item.Key].FirstOrDefault(x => x.Id.Equals(participantId));
                    participant.Name = edited.Name;
                    participant.Surname = edited.Surname;
                    SaveChanges();
                }
            }
        }

        public List<Participant> CopyParticipantsData(List<Participant> targetList, List<Participant> actualList)
        {
            for(int i=0;i<targetList.Count;i++)
            {
                targetList.ElementAt(i).Name = actualList[i].Name;
                targetList.ElementAt(i).Surname = actualList[i].Surname;
                targetList.ElementAt(i).BirthDate = actualList[i].BirthDate;
            }

            return targetList;
        }
    }
}