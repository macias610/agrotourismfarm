using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainModel.Models;
using ViewModel;
using Repository.IRepo;

namespace Service.Service
{
    public class ReservationService : Email, IReservationService
    {
        private readonly IReservationRepository reservationRepository = null;

        public ReservationService(IReservationRepository reservationRepository)
        {
            this.reservationRepository = reservationRepository; 
        }

        public void AddReservation(Reservation reservation)
        {
            this.reservationRepository.AddReservation(reservation);
            this.reservationRepository.SaveChanges();
        }

        public void AddReservationHistory(Reservation_History reservationHistory)
        {
            this.reservationRepository.AddReservationHistory(reservationHistory);
            this.reservationRepository.SaveChanges();
        }

        public void ChangeAssignedAttractions(int id, NewReservation reservation)
        {
            this.reservationRepository.ChangeAssignedAttractions(id, reservation);
        }

        public void ChangeAssignedMeals(int id, NewReservation reservation)
        {
            this.reservationRepository.ChangeAssignedMeals(id, reservation);
        }

        public void ChangeAssignedParticipants(int id, NewReservation reservation)
        {
            this.reservationRepository.ChangeAssignedParticipants(id, reservation);
        }

        public bool checkAvaiabilityHousesBeforeConformation(NewReservation savedReservation)
        {
            bool result = this.reservationRepository.checkAvaiabilityHousesBeforeConformation(savedReservation);
            return result;
        }

        public IList<Attraction> GetAttractionsForReservation(int id)
        {
            IList<Attraction> attractions =  this.reservationRepository.GetAttractionsForReservation(id);
            return attractions;
        }

        public IList<Reservation_History> GetClientArchiveReservations(string id)
        {
            IList<Reservation_History> archiveReservations = this.reservationRepository.GetClientArchiveReservations(id);
            return archiveReservations;
        }

        public IList<Reservation> GetClientReservations(string id)
        {
            IList<Reservation> clientReservations = this.reservationRepository.GetClientReservations(id);
            return clientReservations;
        }

        public Attraction_Reservation GetDetailsAboutReservedAttraction(int id)
        {
            Attraction_Reservation attractionReservation = this.reservationRepository.GetDetailsAboutReservedAttraction(id);
            return attractionReservation;
        }

        public Reservation GetReservationBasedOnData(NewReservation reservation, string userId)
        {
            Reservation res = this.reservationRepository.GetReservationBasedOnData(reservation, userId);
            return res;
        }

        public Reservation GetReservationById(int id)
        {
            Reservation reservation = this.reservationRepository.GetReservationById(id);
            return reservation;
        }

        public Reservation_History GetReservationHistoryBasedReservation(Reservation reservation)
        {
            Reservation_History reservationHistory = this.GetReservationHistoryBasedReservation(reservation);
            return reservationHistory;
        }

        public Reservation_History GetReservationHistoryById(int id)
        {
            Reservation_History reservationHistory = this.reservationRepository.GetReservationHistoryById(id);
            return reservationHistory;
        }

        public IList<Reservation> GetReservationsByState(string state)
        {
            IList<Reservation> reservations = this.reservationRepository.GetReservationsByState(state);
            return reservations;
        }

        public IList<User> GetWorkersAssignedToAttraction(int id)
        {
            IList<User> workers = this.reservationRepository.GetWorkersAssignedToAttraction(id);
            return workers;
        }

        public IList<Reservation> RemoveOutOfDateReservations(IList<Reservation> reservations)
        {
            IList<Reservation> ress = this.reservationRepository.RemoveOutOfDateReservations(reservations);
            return ress;
        }

        public void RemoveReservation(Reservation reservation)
        {
            this.reservationRepository.RemoveReservation(reservation);
            this.reservationRepository.SaveChanges();
        }

        public NewReservation RetreiveExistingReservation(Reservation reservation)
        {
            NewReservation newress = this.reservationRepository.RetreiveExistingReservation(reservation);
            return newress;
        }

        public void SaveAssignedAttractions(int id, NewReservation reservation)
        {
            this.reservationRepository.SaveAssignedAttractions(id, reservation);
        }

        public void SaveAssignedMealsAndHouses(int id, NewReservation reservation)
        {
            this.reservationRepository.SaveAssignedMealsAndHouses(id, reservation);
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
    }
}
