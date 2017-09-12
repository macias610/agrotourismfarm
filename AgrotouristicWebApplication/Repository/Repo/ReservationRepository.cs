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
                participant.Reservation_HouseId = reservationHouseId;
                db.Participants.Add(participant);
                SaveChanges();
            }
        }

        public List<Attraction> GetAttractionsForReservation(int id)
        {
            List<Attraction> attractions = (from attrRes in db.Attractions_Reservations
                                            where attrRes.ReservationId.Equals(id)
                                            join attraction in db.Attractions on attrRes.AttractionId equals attraction.Id
                                            select attraction).ToList();
            return attractions;
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
            Attraction_Reservation attractionReservation = (from attrRes in db.Attractions_Reservations
                                                            where attrRes.AttractionId.Equals(id)
                                                            select attrRes).FirstOrDefault();
            return attractionReservation;
        }

        public Reservation GetReservationById(int id)
        {
            Reservation reservation = db.Reservations.Find(id);
            return reservation;
        }

        public List<User> GetWorkersAssignedToAttraction(int id)
        {
            List<User> workers = (from attrResWor in db.Attractions_Reservations_Workers
                                  where attrResWor.Attraction_ReservationId.Equals(id)
                                  join worker in db.ApplicationUsers on attrResWor.WorkerId equals worker.Id
                                  select worker).ToList();
            return workers;
        }

        public Reservation GetReservationBasedOnData(NewReservation reservation, string userId)
        {
            Reservation savedReservation = new Reservation()
            {
                ClientId = userId,
                DeadlinePayment = DateTime.Now.Date.AddDays(1),
                StartDate = reservation.StartDate,
                EndDate = reservation.EndDate,
                Status = Reservation.States[0],
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
                db.Reservation_Houses.Add(reservationHouse);
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
            List<string> selectedHousesMeals = (from reservationHouse in db.Reservation_Houses
                                                where reservationHouse.ReservationId.Equals(reservation.Id)
                                                join house in db.Houses on reservationHouse.HouseId equals house.Id
                                                join houseType in db.HouseTypes on house.HouseTypeId equals houseType.Id
                                                select (houseType.Type + "(" + house.Name + ");" + reservationHouse.MealId)).ToList();
            foreach (string selectHouseMeal in selectedHousesMeals)
            {
                exisitingReservation.AssignedHousesMeals.Add(selectHouseMeal.Split(';')[0] + ';', Int32.Parse(selectHouseMeal.Split(';')[1]));
            }
            List<string> selectedHousesIdResHouses = (from reservationHouse in db.Reservation_Houses
                                                      where reservationHouse.ReservationId.Equals(reservation.Id)
                                                      join house in db.Houses on reservationHouse.HouseId equals house.Id
                                                      join houseType in db.HouseTypes on house.HouseTypeId equals houseType.Id
                                                      select (houseType.Type + "(" + house.Name + ");" + reservationHouse.Id)).ToList();
            foreach (string selectHouseIdResHou in selectedHousesIdResHouses)
            {
                int reservationHouseId = Int32.Parse(selectHouseIdResHou.Split(';')[1]);
                List<Participant> participants = (from participant in db.Participants
                                                  where participant.Reservation_HouseId.Equals(reservationHouseId)
                                                  select participant).ToList();
                exisitingReservation.AssignedParticipantsHouses.Add(selectHouseIdResHou.Split(';')[0] + ';', participants);
            }
            Dictionary<DateTime, List<string>> dictionary = new Dictionary<DateTime, List<string>>();
            for (DateTime st = reservation.StartDate; st <= reservation.EndDate; st = st.AddDays(1))
            {
                dictionary.Add(st, new List<string>());
            }
            List<Attraction_Reservation> attractionsReservation = (from attrRes in db.Attractions_Reservations
                                                                   where attrRes.ReservationId.Equals(reservation.Id)
                                                                   select attrRes).ToList();
            HashSet<DateTime> dates = new HashSet<DateTime>();
            attractionsReservation.ForEach(item => dates.Add(item.TermAffair));
            foreach (DateTime date in dates)
            {
                List<string> result = new List<string>();
                attractionsReservation.Where(x => x.TermAffair.Equals(date)).ToList().ForEach(x => result.Add(x.Attraction.Name + ',' + x.QuantityParticipant));
                dictionary[date] = result;
            }
            exisitingReservation.AssignedAttractions = dictionary;
            return exisitingReservation;
        }

        public void ChangeAssignedMeals(int id, NewReservation reservation)
        {
            foreach (KeyValuePair<string, int> item in reservation.AssignedHousesMeals)
            {
                string houseName = Regex.Match(item.Key, @"\(([^)]*)\)").Groups[1].Value;
                int houseId = db.Houses.Where(house => house.Name.Equals(houseName)).FirstOrDefault().Id;
                Reservation_House reservationHouse = (from resHouse in db.Reservation_Houses
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

                List<int> participantsId = (from resHouse in db.Reservation_Houses
                                                  where resHouse.ReservationId.Equals(id)
                                                  && resHouse.HouseId.Equals(houseId)
                                                  join participant in db.Participants on resHouse.Id equals participant.Reservation_HouseId
                                                  select participant.Id).ToList();

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

        public void ChangeAssignedAttractions(int id, NewReservation reservation)
        {
            List<Attraction_Reservation> attractionsReservation = (from attrRes in db.Attractions_Reservations
                                                                  where attrRes.ReservationId.Equals(id)
                                                                  select attrRes).ToList();
            Dictionary<DateTime, List<string>> dictionary = new Dictionary<DateTime, List<string>>();
            for(DateTime date=reservation.AssignedAttractions.Keys.First();date<=reservation.AssignedAttractions.Keys.Last();date=date.AddDays(1))
            {
                dictionary.Add(date, new List<string>());
            }
            attractionsReservation.ForEach(item => dictionary[item.TermAffair].Add(item.Attraction.Name + ',' + item.QuantityParticipant));

            List<string> attractionsToRemove = new List<string>();

            foreach(KeyValuePair<DateTime,List<string>> item in dictionary)
            {
                List<Attraction_Reservation> attractionsReservationInDay = attractionsReservation.Where(elem => elem.TermAffair.Equals(item.Key)).ToList();
                List<string> oldAttractions = item.Value;
                List<string> newAttractions = reservation.AssignedAttractions[item.Key];
                List<string> toRemove = oldAttractions.Where(elem =>!(newAttractions.Contains(elem))).ToList();
                foreach(string attractionToRemove in toRemove)
                {
                    Attraction_Reservation attractionReservation = attractionsReservationInDay.Where(elem => elem.Attraction.Name.Equals(attractionToRemove.Split(',')[0])).FirstOrDefault();
                    attractionsReservationInDay.Remove(attractionReservation);
                    db.Attractions_Reservations.Remove(attractionReservation);
                }
            }
            SaveChanges();
            foreach (KeyValuePair<DateTime, List<string>> item in reservation.AssignedAttractions.Where(pair => pair.Value.Any()))
            {
                List<string> oldAttractions = dictionary[item.Key];
                List<string> newAttractions = item.Value;
                List<string> toAdd = newAttractions.Where(elem => !(oldAttractions.Contains(elem))).ToList();
                foreach(string attractionToAdd in toAdd)
                {
                    string attractionName = attractionToAdd.Split(',')[0];
                    Attraction attraction = (from attr in db.Attractions
                                             where attr.Name.Equals(attractionName)
                                             select attr).FirstOrDefault();
                    Attraction_Reservation attractionReservation = new Attraction_Reservation()
                    {
                        AttractionId = attraction.Id,
                        ReservationId=id,
                        TermAffair=item.Key,
                        QuantityParticipant = Int32.Parse(attractionToAdd.Split(',')[1]),
                        OverallCost = attraction.Price * Int32.Parse(attractionToAdd.Split(',')[1])
                    };
                    db.Attractions_Reservations.Add(attractionReservation);
                }
            }
            Reservation editedReservation = GetReservationById(id);
            editedReservation.OverallCost = reservation.OverallCost;
            db.Entry(editedReservation).State = EntityState.Modified;
            SaveChanges();
        }

        public void RemoveReservation(Reservation reservation)
        {
            db.Reservations.Remove(reservation);
        }   

        public List<Reservation> RemoveOutOfDateReservations(List<Reservation> reservations)
        {
            List<Reservation> result = new List<Reservation>();
            foreach(Reservation reservation in reservations)
            {
                if(DateTime.Now.Date.CompareTo(reservation.DeadlinePayment)>0 && reservation.Status.Equals("oczekiwanie"))
                {
                    RemoveReservation(reservation);
                }
                else
                {
                    result.Add(reservation);
                }
            }
            SaveChanges();
            return result;
        }

        public IQueryable<Reservation> GetReservationsByState(string state)
        {
            IQueryable<Reservation> reservations = null;
            if (state.Equals("wszystkie"))
            {
                reservations = db.Reservations.AsNoTracking();
            }
            else
            {
                reservations = db.Reservations.Where(x => x.Status.Equals(state)).AsNoTracking();
            }
            return reservations;
        }

        public void UpdateReservation(Reservation reservation)
        {
            db.Entry(reservation).State = EntityState.Modified;
        }

        public void AddReservationHistory(Reservation_History reservationHistory)
        {
            db.Reservations_History.Add(reservationHistory);
        }

        public Reservation_History GetReservationHistoryBasedReservation(Reservation reservation)
        {
            string reservedHouses=String.Empty;
            string reservedAttractions = String.Empty;

            foreach(Reservation_House reservationHouse in reservation.Reservation_House.ToList())
            {
                reservedHouses += reservationHouse.House.HouseType.Type + "(" + reservationHouse.House.Name + ");";
            }
            reservedHouses = reservedHouses.Remove(reservedHouses.Length - 1);

            if (reservation.Attraction_Reservation.Count > 0)
            {
                HashSet<string> set = new HashSet<string>();
                foreach (Attraction_Reservation reservationAttraction in reservation.Attraction_Reservation.ToList())
                {
                    set.Add(reservationAttraction.Attraction.Name);
                }
                foreach (string item in set)
                {
                    reservedAttractions += item + ';';
                }
                reservedAttractions = reservedAttractions.Remove(reservedAttractions.Length - 1);
            }
            else
                reservedAttractions = "Brak";
            
            Reservation_History reservationHistory = new Reservation_History()
            {
                ClientId=reservation.ClientId,
                StartDate=reservation.StartDate,
                EndDate=reservation.EndDate,
                OverallCost=reservation.OverallCost,
                ReservedHouses= reservedHouses,
                ReservedAttractions =reservedAttractions
            };
            return reservationHistory;
        }

        public IQueryable<Reservation_History> GetClientArchiveReservations(string id)
        {
            IQueryable<Reservation_History> archiveReservations = (from archiveReservation in db.Reservations_History
                                                   where archiveReservation.ClientId.Equals(id)
                                                   select archiveReservation).AsNoTracking();
            return archiveReservations;
        }

        public Reservation_History GetReservationHistoryById(int id)
        {
            Reservation_History reservationHistory = db.Reservations_History.Find(id);
            return reservationHistory;
        }

        public void SaveAssignedAttractions(int id, NewReservation reservation)
        {
            Dictionary<DateTime, List<string>> dictionary = reservation.AssignedAttractions.Where(x => x.Value.Any()).ToDictionary(t => t.Key, t => t.Value);

            foreach(KeyValuePair<DateTime,List<string>> item in dictionary)
            {
                foreach(string attr in item.Value)
                {
                    string attractionName = attr.Split(',')[0];
                    int quantityParticipants = Int32.Parse(attr.Split(',')[1]);
                    Attraction attraction = (from attract in db.Attractions
                                             where attract.Name.Equals(attractionName)
                                             select attract).FirstOrDefault();
                    Attraction_Reservation attractionReservation = new Attraction_Reservation()
                    {
                        AttractionId = attraction.Id,
                        ReservationId = id,
                        TermAffair = item.Key,
                        QuantityParticipant = quantityParticipants,
                        OverallCost = quantityParticipants*attraction.Price
                    };
                    db.Attractions_Reservations.Add(attractionReservation);
                }
                SaveChanges();
            }
        }

    }
}