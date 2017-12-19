using Repository.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DomainModel.Models;
using System.Data.Entity;

namespace Repository.Repo
{
    public class AttractionReservationRepository : IAttractionReservationRepository
    {
        private readonly IAgrotourismContext db;

        public AttractionReservationRepository(IAgrotourismContext db)
        {
            this.db = db;
        }

        public void AddAttractionReservation(Attraction_Reservation attractionReservation)
        {
            db.Attractions_Reservations.Add(attractionReservation);
        }

        public Attraction_Reservation GetAttractionReservationById(int id)
        {
            Attraction_Reservation attractionReservation = db.Attractions_Reservations.Find(id);
            return attractionReservation;
        }

        public IList<Attraction_Reservation> GetAttractionsReservations()
        {
            IQueryable<Attraction_Reservation> attractionReservations = db.Attractions_Reservations.AsNoTracking();
            return attractionReservations.ToList();
        }

        public void RemoveAttractionReservation(Attraction_Reservation attractionReservation)
        {
            db.Entry(attractionReservation).State = EntityState.Deleted;
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }

        public void UpdateAttractionReservation(Attraction_Reservation attractionReservation, byte[] rowVersion)
        {
            db.Entry(attractionReservation).OriginalValues["RowVersion"] = rowVersion;
        }
    }
}