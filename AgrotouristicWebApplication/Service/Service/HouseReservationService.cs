using DomainModel.Models;
using Repository.IRepo;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ViewModel;

namespace Service.Service
{
    public class HouseReservationService : IHouseReservationService
    {
        private readonly IReservationRepository reservationRepository = null;
        private readonly IHouseRepository houseRepository = null;
        private readonly IReservationHouseRepository reservationHouseRepository = null;

        public HouseReservationService(IReservationRepository reservationRepository, IHouseRepository houseRepository, IReservationHouseRepository reservationHouseRepository)
        {
            this.reservationRepository = reservationRepository;
            this.houseRepository = houseRepository;
            this.reservationHouseRepository = reservationHouseRepository;
        }

        public void ConfirmSelectedHouses(NewReservation reservation, IList<string> selectedHouses)
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

        public IList<House> GetAvaiableHousesInTerm(DateTime startDate, DateTime endDate)
        {
            IList<Reservation> reservations = this.reservationRepository.GetReservations();
            reservations = reservations.Where(reservation => startDate.CompareTo(reservation.StartDate) >= 0 & startDate.CompareTo(reservation.EndDate) < 0 ||
                                                             endDate.CompareTo(reservation.StartDate) >= 0 & endDate.CompareTo(reservation.EndDate) < 0 ||
                                                             reservation.StartDate.CompareTo(startDate) >= 0 & reservation.StartDate.CompareTo(endDate) < 0 ||
                                                             reservation.EndDate.CompareTo(startDate) >= 0 & reservation.EndDate.CompareTo(endDate) < 0)
                                        .ToList();
            List<int> housesId = new List<int>();
            foreach (Reservation reservation in reservations)
            {
                reservation.Reservation_House.ToList().ForEach(item => housesId.Add(item.HouseId));
            }
            List<House> houses = this.houseRepository.GetHouses().ToList();
            houses.RemoveAll(item => housesId.Contains(item.Id));
            return houses;
        }

        public House GetHouseByName(string name)
        {
            IList<House> houses = this.houseRepository.GetHouses();
            House house = houses.Where(item => item.Name.Equals(name))
                            .SingleOrDefault();
            return house;
        }

        public IList<House> GetHousesForReservation(int id)
        {
            IList<Reservation_House> reservationHouses = this.reservationHouseRepository
                                                            .GetReservationHousesOfReservationId(id);
            IList<House> houses = reservationHouses.Select(item => item.House).ToList();
            return houses;
        }

        public IList<SelectListItem> GetNamesAvaiableHouses(IList<House> houses)
        {
            List<SelectListItem> selectList = houses.Select(house => new SelectListItem { Value = house.HouseType.Type + "(" + house.Name + ")|" + "(" + house.HouseType.Price + "[zł]/doba)" + ";", Text = house.HouseType.Type + "(" + house.Name + ")" + ";" })
                                                .ToList();
            return selectList;
        }
    }
}
