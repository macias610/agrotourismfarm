using Repository.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DomainModel.Models;
using System.Data.Entity;

namespace Repository.Repo
{
    public class ReservationHouseRepository : IReservationHouseRepository
    {
        private readonly IAgrotourismContext db;

        public ReservationHouseRepository(IAgrotourismContext db)
        {
            this.db = db;
        }

        public void AddReservationHouse(ReservationHouse reservationHouse)
        {
            db.Reservation_Houses.Add(reservationHouse);
        }

        public ReservationHouse GetReservationHouseById(int id)
        {
            ReservationHouse reservationHouse = this.db.Reservation_Houses.Find(id);
            return reservationHouse;
        }

        public IList<ReservationHouse> GetReservationHousesOfReservationId(int id)
        {
            IList<ReservationHouse> reservationHouses = (from resHouse in db.Reservation_Houses
                                                          where resHouse.ReservationId.Equals(id)
                                                          select resHouse).ToList();
            return reservationHouses;
        }

        public IList<ReservationHouse> GetReservationsHouses()
        {
            IQueryable<ReservationHouse> reservationsHouses = this.db.Reservation_Houses.AsNoTracking();
            return reservationsHouses.ToList();
        }

        public void RemoveReservationHouse(ReservationHouse reservationHouse)
        {
            this.db.Entry(reservationHouse).State = EntityState.Deleted;
        }

        public void SaveChanges()
        {
            this.db.SaveChanges();
        }

        public void UpdateReservationHouse(ReservationHouse reservationHouse, byte[] rowVersion)
        {
            db.Entry(reservationHouse).OriginalValues["RowVersion"] = rowVersion;
        }
    }
}