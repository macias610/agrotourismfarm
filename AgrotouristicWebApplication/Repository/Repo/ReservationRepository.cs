using Repository.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using DomainModel.Models;
using ViewModel;

namespace Repository.Repo
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly IAgrotourismContext db;

        public ReservationRepository(IAgrotourismContext db)
        {
            this.db = db;
        }

        public Reservation GetReservationById(int id)
        {
            Reservation reservation = db.Reservations.Find(id);
            return reservation;
        }

        public void AddReservation(Reservation reservation)
        {
            db.Reservations.Add(reservation);
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }

        public void RemoveReservation(Reservation reservation)
        {
            Reservation toDelete = GetReservationById(reservation.Id);
            if(!reservation.RowVersion.SequenceEqual(toDelete.RowVersion))
            {
                throw new DbUpdateConcurrencyException();
            }
            db.Reservations.Remove(toDelete);
        }   

        public IList<Reservation> GetReservationsByState(string state)
        {
            IQueryable<Reservation> reservations = null;
            if (state.Equals("wszystkie"))
            {
                reservations = db.Reservations.AsNoTracking();
            }
            else
            {
                reservations = db.Reservations.Where(x => x.Status.Equals(state)).AsNoTracking();
            }
            return reservations.ToList();
        }

        public void UpdateReservation(Reservation reservation,byte[] rowVersion)
        {
            db.Entry(reservation).OriginalValues["RowVersion"] = rowVersion;
        }

        public void AddReservationHistory(ReservationHistory reservationHistory)
        {
            db.Reservations_History.Add(reservationHistory);
        }

        public IList<ReservationHistory> GetClientArchiveReservations(string id)
        {
            IQueryable<ReservationHistory> archiveReservations = (from archiveReservation in db.Reservations_History
                                                   where archiveReservation.ClientId.Equals(id)
                                                   select archiveReservation).AsNoTracking();
            return archiveReservations.ToList();
        }

        public ReservationHistory GetReservationHistoryById(int id)
        {
            ReservationHistory reservationHistory = db.Reservations_History.Find(id);
            return reservationHistory;
        }

        public User GetClientById(string id)
        {
            User user = db.ApplicationUsers.Find(id);
            return user;
        }

        public IList<Reservation> GetReservations()
        {
            IQueryable<Reservation> reservations = this.db.Reservations.AsNoTracking();
            return reservations.ToList();
        }
    }
}