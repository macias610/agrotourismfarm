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

        public void AddAttractionReservation(AttractionReservation attractionReservation)
        {
            db.Attractions_Reservations.Add(attractionReservation);
        }

        public AttractionReservation GetAttractionReservationById(int id)
        {
            AttractionReservation attractionReservation = db.Attractions_Reservations.Find(id);
            return attractionReservation;
        }

        public IList<AttractionReservation> GetAttractionsReservations()
        {
            IQueryable<AttractionReservation> attractionReservations = db.Attractions_Reservations.AsNoTracking();
            return attractionReservations.ToList();
        }

        public IList<AttractionReservation> GetAttractionsReservationsByReservationId(int id)
        {
            IList<AttractionReservation> attractionsReservation = (from attrRes in db.Attractions_Reservations
                                                                    where attrRes.ReservationId.Equals(id)
                                                                    select attrRes).ToList();
            return attractionsReservation;
        }

        public void RemoveAttractionReservation(AttractionReservation attractionReservation)
        {
            db.Entry(attractionReservation).State = EntityState.Deleted;
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }

        public void UpdateAttractionReservation(AttractionReservation attractionReservation, byte[] rowVersion)
        {
            db.Entry(attractionReservation).OriginalValues["RowVersion"] = rowVersion;
        }
    }
}