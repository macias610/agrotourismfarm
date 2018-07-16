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

        public void AddReservationHistory(ReservationHistory reservationHistory)
        {
            this.reservationRepository.AddReservationHistory(reservationHistory);
            this.reservationRepository.SaveChanges();
        }

        public IList<ReservationHistory> GetClientArchiveReservations(string id)
        {
            IList<ReservationHistory> archiveReservations = this.reservationRepository.GetClientArchiveReservations(id);
            return archiveReservations;
        }

        public ReservationHistory GetReservationHistoryBasedReservation(Reservation reservation)
        {
            string reservedHouses = String.Empty;
            string reservedAttractions = String.Empty;

            foreach (ReservationHouse reservationHouse in reservation.Reservation_House.ToList())
            {
                reservedHouses += reservationHouse.House.HouseType.Type + "(" + reservationHouse.House.Name + ");";
            }
            reservedHouses = reservedHouses.Remove(reservedHouses.Length - 1);

            if (reservation.Attraction_Reservation.Count > 0)
            {
                HashSet<string> set = new HashSet<string>();
                foreach (AttractionReservation reservationAttraction in reservation.Attraction_Reservation.ToList())
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

            ReservationHistory reservationHistory = new ReservationHistory()
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

        public ReservationHistory GetReservationHistoryById(int id)
        {
            ReservationHistory reservationHistory = this.reservationRepository.GetReservationHistoryById(id);
            return reservationHistory;
        }
    }
}
