using DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepo
{
    public interface IReservationHouseRepository
    {
        void AddReservationHouse(Reservation_House reservationHouse);

        Reservation_House GetReservationHouseById(int id);

        IList<Reservation_House> GetReservationsHouses();

        void RemoveReservationHouse(Reservation_House reservationHouse);

        void SaveChanges();

        void UpdateReservationHouse(Reservation_House reservationHouse, byte[] rowVersion);
    }
}
