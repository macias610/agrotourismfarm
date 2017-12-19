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

        public void AddReservationHouse(Reservation_House reservationHouse)
        {
            db.Reservation_Houses.Add(reservationHouse);
        }

        public Reservation_House GetReservationHouseById(int id)
        {
            Reservation_House reservationHouse = this.db.Reservation_Houses.Find(id);
            return reservationHouse;
        }

        public IList<Reservation_House> GetReservationsHouses()
        {
            IQueryable<Reservation_House> reservationsHouses = this.db.Reservation_Houses.AsNoTracking();
            return reservationsHouses.ToList();
        }

        public void RemoveReservationHouse(Reservation_House reservationHouse)
        {
            this.db.Entry(reservationHouse).State = EntityState.Deleted;
        }

        public void SaveChanges()
        {
            this.db.SaveChanges();
        }

        public void UpdateReservationHouse(Reservation_House reservationHouse, byte[] rowVersion)
        {
            db.Entry(reservationHouse).OriginalValues["RowVersion"] = rowVersion;
        }
    }
}