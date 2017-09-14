using Repository.IRepo;
using Repository.Models;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
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

        public Dictionary<DateTime, List<string>> InitializeDictionaryForAssignedAttractions(DateTime startDate, DateTime endDate)
        {
            Dictionary<DateTime, List<string>> result = new Dictionary<DateTime, List<string>>();
            for(DateTime start=startDate;start.CompareTo(endDate)<=0;start=start.AddDays(1))
            {
                result.Add(start, new List<string>());
            }
            return result;
        }

        public List<SelectListItem> GetWeeksFromSelectedTerm(DateTime startDate, DateTime endDate)
        {
            List<string> list = new List<string>();
            list.Add("-");
            int counter = 0;
            int daysToDisplay = 6;
            for (DateTime start = startDate, tmp = startDate;tmp.CompareTo(endDate)<=0;tmp=tmp.AddDays(1),counter++)
            {
                if(counter==daysToDisplay || tmp.CompareTo(endDate)==0)
                {
                    counter = 0;
                    list.Add(start.ToShortDateString() + ";" + tmp.ToShortDateString());
                    start = tmp.AddDays(1);
                }
            }
            List<SelectListItem> selectList = list.Select(item => new SelectListItem { Text = item, Value = item, Selected = item.Equals("-") ? true : false }).ToList();
            return selectList;
        }

        public List<SelectListItem> GetAvaiableDatesInWeek(string term)
        {
            List<DateTime> result = new List<DateTime>();
            for (DateTime start = DateTime.Parse(term.Split(';')[0]); start.CompareTo(DateTime.Parse(term.Split(';')[1])) <= 0; start = start.AddDays(1))
            {
                result.Add(start);
            }
            List<SelectListItem> selectList = result.Select(item => new SelectListItem { Text = item.ToShortDateString(), Value = item.ToShortDateString(), Selected = true }).ToList();
            return selectList;
        }

        public List<SelectListItem> GetAvaiableAttractions()
        {
            List<string> namesAttractions = (from attraction in db.Attractions
                                             select attraction.Name).ToList();
            List<SelectListItem> selectList = namesAttractions.Select(item => new SelectListItem { Text = item, Value = item, Selected = true }).ToList();
            return selectList;
        }

        public List<SelectListItem> GetParticipantsQuantity(int quantity)
        {
            List<string> list = new List<string>();
            for(int i=2;i<=quantity;i++)
            {
                list.Add(i.ToString());
            }
            List<SelectListItem> selectList = list.Select(item => new SelectListItem { Text = item, Value = item, Selected = true }).ToList();
            return selectList;
        }

        public Dictionary<DateTime, List<string>> GetAttractionsInGivenWeek(string term, Dictionary<DateTime, List<string>> dictionary)
        {
            Dictionary<DateTime, List<string>> result = new Dictionary<DateTime, List<string>>();
            for (DateTime start = DateTime.Parse(term.Split(';')[0]); start.CompareTo(DateTime.Parse(term.Split(';')[1])) <= 0; start = start.AddDays(1))
            {
                result.Add(start, dictionary[start]);
            }
            return result;
        }


        public int GetMaxRowsToTableAttractions(Dictionary<DateTime, List<string>> dictionary)
        {
            int result = 0;
            foreach(KeyValuePair<DateTime,List<string>> item in dictionary)
            {
                if(item.Value.Count>result)
                {
                    result = item.Value.Count;
                }
            }
            if(result==0)
            {
                result++;
            }
            return result;
        }

        public Reservation GetReservationById(int id)
        {
            Reservation reservation = db.Reservations.Find(id);
            return reservation;
        }

        public Dictionary<DateTime, List<string>> RetreiveAttractionsInGivenWeek(string term, int id)
        {
            DateTime start = DateTime.Parse(term.Split(';')[0]);
            DateTime end = DateTime.Parse(term.Split(';')[1]);
            List<Attraction_Reservation> attractionsReservation = (from attrRes in db.Attractions_Reservations
                                                                   where attrRes.ReservationId.Equals(id)
                                                                   && attrRes.TermAffair.CompareTo(start) >= 0
                                                                   && attrRes.TermAffair.CompareTo(end) <= 0
                                                                   select attrRes).ToList();

            Dictionary<DateTime, List<string>> dictionary = new Dictionary<DateTime, List<string>>();
            for(DateTime st=start;st<=end;st=st.AddDays(1))
            {
                dictionary.Add(st, new List<string>());
            }
            HashSet<DateTime> dates = new HashSet<DateTime>();
            attractionsReservation.ForEach(item => dates.Add(item.TermAffair));
            foreach(DateTime date in dates)
            {
                List<string> result = new List<string>();
                attractionsReservation.Where(x => x.TermAffair.Equals(date)).ToList().ForEach(x => result.Add(x.Attraction.Name + ',' + x.QuantityParticipant));
                dictionary[date]=result;
            }
            return dictionary;
        }

        public Dictionary<string, List<Participant>> RetreiveHouseParticipants(int id)
        {
            List<Reservation_House> reservationHouses = (from resHou in db.Reservation_Houses
                                                         where resHou.ReservationId.Equals(id)
                                                         select resHou).ToList();
            Dictionary<string, List<Participant>> dictionary = new Dictionary<string, List<Participant>>();
            foreach(Reservation_House resHouse in reservationHouses)
            {
                string houseName = resHouse.House.HouseType.Type + '(' + resHouse.House.Name + ");";
                List<Participant> participants = resHouse.Participant.ToList();
                dictionary.Add(houseName, participants);
            }
            return dictionary;
        }

        public Attraction GetAttractionByName(string name)
        {
            Models.Attraction attraction = (from attr in db.Attractions
                                     where attr.Name.Equals(name)
                                     select attr).FirstOrDefault();
            return attraction;
        }
    }
}