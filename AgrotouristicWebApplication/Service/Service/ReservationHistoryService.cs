using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainModel.Models;
using Repository.IRepo;

namespace Service.Service
{
    public class ReservationHistoryService : IReservationHistoryService
    {
        private readonly IReservationRepository reservationRepository = null;

        public ReservationHistoryService(IReservationRepository reservationRepository)
        {
            this.reservationRepository = reservationRepository;
        }

        public void AddReservationHistory(Reservation_History reservationHistory)
        {
            this.reservationRepository.AddReservationHistory(reservationHistory);
            this.reservationRepository.SaveChanges();
        }

        public IList<Reservation_History> GetClientArchiveReservations(string id)
        {
            IList<Reservation_History> archiveReservations = this.reservationRepository.GetClientArchiveReservations(id);
            return archiveReservations;
        }

        public Reservation_History GetReservationHistoryBasedReservation(Reservation reservation)
        {
            string reservedHouses = String.Empty;
            string reservedAttractions = String.Empty;

            foreach (Reservation_House reservationHouse in reservation.Reservation_House.ToList())
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
                ClientId = reservation.ClientId,
                StartDate = reservation.StartDate,
                EndDate = reservation.EndDate,
                OverallCost = reservation.OverallCost,
                ReservedHouses = reservedHouses,
                ReservedAttractions = reservedAttractions
            };
            return reservationHistory;
        }

        public Reservation_History GetReservationHistoryById(int id)
        {
            Reservation_History reservationHistory = this.reservationRepository.GetReservationHistoryById(id);
            return reservationHistory;
        }
    }
}
