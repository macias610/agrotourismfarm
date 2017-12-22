using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainModel.Models;
using System.Web.Mvc;
using Repository.IRepo;

namespace Service.Service
{
    public class HouseService : IHouseService
    {
        private readonly IHouseRepository houseRepository = null;
        private readonly IReservationRepository reservationRepository = null;
        private readonly IReservationHouseRepository reservationHouseRepository = null;

        public HouseService(IHouseRepository houseRepository, IReservationRepository reservationRepository, IReservationHouseRepository reservationHouseRepository)
        {
            this.houseRepository = houseRepository;
            this.reservationRepository = reservationRepository;
            this.reservationHouseRepository = reservationHouseRepository;
        }

        public void AddHouse(House house)
        {
            this.houseRepository.AddHouse(house);
            this.houseRepository.SaveChanges();
        }

        public House GetHouseById(int id)
        {
            House house = this.houseRepository.GetHouseById(id);
            return house;
        }

        public IList<House> GetHouses()
        {
            IList<House> houses = this.houseRepository.GetHouses();
            return houses;
        }

        public void RemoveHouse(House house)
        {
            this.houseRepository.RemoveHouse(house);
            this.houseRepository.SaveChanges();

        }

        public void setAvailabilityHouse(House house)
        {
            IList<Reservation> reservations = this.reservationRepository.GetReservations();
            IList<Reservation_House> reservationsHouses = this.reservationHouseRepository
                                                                .GetReservationsHouses();
            IList<int> reservationsIdOfHouse = reservationsHouses
                                                .Where(item => item.HouseId.Equals(house.Id))
                                                .Select(item => item.ReservationId).ToList();

            reservations = reservations.Where(item => reservationsIdOfHouse.Contains(item.Id))
                            .Where(item => item.StartDate <= DateTime.Now)
                            .Where(item => item.EndDate >= DateTime.Now)
                            .ToList();

            if (reservations.Count >= 1)
            {
                house.statusHouse = "Zajęty";
            }
            else if (reservationsIdOfHouse.Count >= 1)
            {
                house.statusHouse = "Zarezerwowany";
            }
            else
            {
                house.statusHouse = "Wolny";
            }
        }

        public void UpdateHouse(House house, byte[] rowVersion)
        {
            this.houseRepository.UpdateHouse(house, rowVersion);
            this.houseRepository.SaveChanges();
        }

    }
}
