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
    }
}