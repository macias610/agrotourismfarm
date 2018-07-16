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
        void AddReservationHouse(ReservationHouse reservationHouse);

        ReservationHouse GetReservationHouseById(int id);

        IList<ReservationHouse> GetReservationsHouses();

        IList<ReservationHouse> GetReservationHousesOfReservationId(int id);

        void RemoveReservationHouse(ReservationHouse reservationHouse);

        void SaveChanges();

        void UpdateReservationHouse(ReservationHouse reservationHouse, byte[] rowVersion);
    }
}
