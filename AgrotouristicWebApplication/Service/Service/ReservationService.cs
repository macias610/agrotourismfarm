using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainModel.Models;
using ViewModel;
using Repository.IRepo;
using System.Transactions;
using System.Text.RegularExpressions;

namespace Service.Service
{
    public class ReservationService : Email, IReservationService
    {
        private readonly IReservationRepository reservationRepository = null;
        private readonly IAttractionReservationRepository attractionReservationRepository = null;
        private readonly IAttractionRepository attractionRepository = null;
        private readonly IParticipantRepository participantRepository = null;
        private readonly IReservationHouseRepository reservationHouseRepository = null;
        private readonly IHouseRepository houseRepository = null;

        public ReservationService(IReservationRepository reservationRepository, IAttractionReservationRepository attractionReservationRepository, IAttractionRepository attractionRepository, IParticipantRepository participantRepository, IReservationHouseRepository reservationHouseRepository, IHouseRepository houseRepository)
        {
            this.reservationRepository = reservationRepository;
            this.attractionReservationRepository = attractionReservationRepository;
            this.attractionRepository = attractionRepository;
            this.participantRepository = participantRepository;
            this.reservationHouseRepository = reservationHouseRepository;
            this.houseRepository = houseRepository;
        }

        public void AddReservation(Reservation reservation)
        {
            this.reservationRepository.AddReservation(reservation);
            this.reservationRepository.SaveChanges();
        }

        public bool checkAvaiabilityHousesBeforeConfirmation(NewReservation savedReservation)
        {
            DateTime startDate = savedReservation.StartDate;
            DateTime endDate = savedReservation.EndDate;
            IList<int> reservationsId = this.reservationRepository.GetReservations()
                                                        .Where(reservation => startDate.CompareTo(reservation.StartDate) >= 0 & startDate.CompareTo(reservation.EndDate) < 0 ||
                                                                              endDate.CompareTo(reservation.StartDate) >= 0 & endDate.CompareTo(reservation.EndDate) < 0 ||
                                                                              reservation.StartDate.CompareTo(startDate) >= 0 & reservation.StartDate.CompareTo(endDate) < 0 ||
                                                                              reservation.EndDate.CompareTo(startDate) >= 0 & reservation.EndDate.CompareTo(endDate) < 0
                                                        )
                                                        .Select(item => item.Id)
                                                        .ToList();
            IList<ReservationHouse> reservationHouses = this.reservationHouseRepository.GetReservationsHouses();
            IList<string> reservationHousesNames = reservationHouses
                                                    .Where(item => reservationsId.Contains(item.ReservationId))
                                                    .Select(item => item.House.Name)
                                                    .ToList();

            List<string> housesName = this.houseRepository.GetHouses()
                                            .Select(item => item.Name)
                                            .ToList();
            housesName.RemoveAll(item => reservationHousesNames.Contains(item));
            List<string> selectedHouses = new List<string>();
            savedReservation.AssignedParticipantsHouses.Keys.ToList()
                .ForEach(item => selectedHouses.Add(Regex.Match(item, @"\(([^)]*)\)").Groups[1].Value));
            foreach (string houseName in selectedHouses)
            {
                if (!housesName.Contains(houseName))
                {
                    return false;
                }
            }
            return true;
        }

        public IList<Reservation> GetClientReservations(string id)
        {
            IList<Reservation> clientReservations = this.reservationRepository
                                                            .GetReservations()
                                                            .Where(item => item.ClientId.Equals(id))
                                                            .ToList();
            return clientReservations;
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

        public Reservation GetReservationById(int id)
        {
            Reservation reservation = this.reservationRepository.GetReservationById(id);
            return reservation;
        }
        
        public IList<Reservation> GetReservationsByState(string state)
        {
            IList<Reservation> reservations = this.reservationRepository.GetReservationsByState(state);
            return reservations;
        }

        public void RemoveOutOfDateReservations(string userId)
        {
            IList<Reservation> reservations = this.GetClientReservations(userId);
            using (TransactionScope scope = new TransactionScope())
            {
                foreach (Reservation reservation in reservations)
                {
                    if (DateTime.Now.Date.CompareTo(reservation.DeadlinePayment) > 0 && reservation.Status.Equals("oczekiwanie"))
                    {
                        this.reservationRepository.RemoveReservation(reservation);
                        this.reservationRepository.SaveChanges();
                    }
                }
                scope.Complete();
            }
        }

        public void RemoveReservation(Reservation reservation)
        {
            this.reservationRepository.RemoveReservation(reservation);
            this.reservationRepository.SaveChanges();
        }

        public NewReservation RetreiveExistingReservation(Reservation reservation)
        {
            NewReservation exisitingReservation = new NewReservation();
            exisitingReservation.StartDate = reservation.StartDate;
            exisitingReservation.EndDate = reservation.EndDate;
            exisitingReservation.OverallCost = reservation.OverallCost;
            IList<ReservationHouse> reservationHouses = this.reservationHouseRepository
                                                                .GetReservationHousesOfReservationId(reservation.Id);
            List<string> selectedHousesMeals = reservationHouses
                                                .Select(item => item.House.HouseType.Type + "(" + item.House.Name + ");" + item.MealId)
                                                .ToList();
            foreach (string selectHouseMeal in selectedHousesMeals)
            {
                exisitingReservation.AssignedHousesMeals.Add(selectHouseMeal.Split(';')[0] + ';', Int32.Parse(selectHouseMeal.Split(';')[1]));
            }
            IList<string> selectedHousesIdResHouses = reservationHouses
                                                .Select(item => item.House.HouseType.Type + "(" + item.House.Name + ");" + item.Id)
                                                .ToList();
            foreach (string selectHouseIdResHou in selectedHousesIdResHouses)
            {
                int reservationHouseId = Int32.Parse(selectHouseIdResHou.Split(';')[1]);
                List<Participant> participants = this.participantRepository
                                                        .GetParticipantsForHouseReservation(reservationHouseId)
                                                        .ToList();
                exisitingReservation.AssignedParticipantsHouses.Add(selectHouseIdResHou.Split(';')[0] + ';', participants);
            }
            Dictionary<DateTime, List<string>> dictionary = new Dictionary<DateTime, List<string>>();
            for (DateTime st = reservation.StartDate; st <= reservation.EndDate; st = st.AddDays(1))
            {
                dictionary.Add(st, new List<string>());
            }
            List<AttractionReservation> attractionsReservation = this.attractionReservationRepository
                                                                        .GetAttractionsReservationsByReservationId(reservation.Id)
                                                                        .ToList();
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

        public void SaveReservationHouses(int id, NewReservation reservation)
        {
            foreach (KeyValuePair<string, int> item in reservation.AssignedHousesMeals)
            {
                string houseName = Regex.Match(item.Key, @"\(([^)]*)\)").Groups[1].Value;
                int houseId = this.houseRepository.GetHouseByName(houseName).Id;
                ReservationHouse reservationHouse = new ReservationHouse()
                {
                    HouseId = houseId,
                    MealId = item.Value,
                    ReservationId = id
                };
                this.reservationHouseRepository.AddReservationHouse(reservationHouse);
                this.reservationHouseRepository.SaveChanges();
                this.SaveAssignedHouseParticipants(reservationHouse.Id, reservation.AssignedParticipantsHouses[item.Key]);

            }
        }

        public void SendEmailAwaitingReservation(Reservation reservation)
        {
            reservation.Client = this.reservationRepository.GetClientById(reservation.ClientId);
            string subject = "Złożenie rezerwacji";
            string body = string.Format("Drogi {0},<BR/> dziękujemy za złożenie rezerwacji<BR/>"
                + "Data przyjazdu: {1}<BR/>"
                + "Data wyjazdu: {2}<BR/>"
                + "Termin płatności: {3}<BR/>"
                + "Koszt całkowity: {4} zł<BR/>"
                + "Prosimy do dokonanie wpłaty na konto podane na stronie głównej.", reservation.Client.UserName, reservation.StartDate.ToShortDateString(), reservation.EndDate.ToShortDateString(), reservation.DeadlinePayment.ToShortDateString(), reservation.OverallCost);
            base.SendEmail(reservation.Client.UserName, subject, body);
        }

        public void SendEmailConfirmingReservation(Reservation reservation)
        {
            reservation.Client = this.reservationRepository.GetClientById(reservation.ClientId);
            string subject = "Potwierdzenie rezerwacji";
            string body = string.Format("Drogi {0},<BR/> dziękujemy za dokonanie wpłaty rezerwacji<BR/>"
                + "Data przyjazdu: {1}<BR/>"
                + "Data wyjazdu: {2}<BR/>"
                + "Termin płatności: {3}<BR/>"
                + "Koszt całkowity: {4} zł<BR/>", reservation.Client.UserName, reservation.StartDate.ToShortDateString(), reservation.EndDate.ToShortDateString(), reservation.DeadlinePayment.ToShortDateString(), reservation.OverallCost);
            base.SendEmail(reservation.Client.UserName, subject, body);
        }

        public void UpdateReservation(Reservation reservation, byte[] rowVersion)
        {
            this.reservationRepository.UpdateReservation(reservation, rowVersion);
            this.reservationRepository.SaveChanges();
        }

        private void SaveAssignedHouseParticipants(int reservationHouseId, List<Participant> participants)
        {
            using (TransactionScope scope2 = new TransactionScope())
            {
                foreach (Participant participant in participants)
                {
                    participant.Reservation_HouseId = reservationHouseId;
                    this.participantRepository.AddParticipant(participant);
                    this.participantRepository.SaveChanges();
                }
                scope2.Complete();
            }   
        }
    }
}
