using Repository.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DomainModel.Models;
using System.Data.Entity;

namespace Repository.Repo
{
    public class AttractionReservationWorkerRepository : IAttractionReservationWorkerRepository
    {
        private readonly IAgrotourismContext db = null;

        public AttractionReservationWorkerRepository(IAgrotourismContext db)
        {
            this.db = db;
        }

        public void AddAttractionReservationWorker(AttractionReservationWorker attractionReservationWorker)
        {
            this.db.Attractions_Reservations_Workers.Add(attractionReservationWorker);
        }

        public AttractionReservationWorker GetAttractionReservationWorkerById(int id)
        {
            var attrResWorker = this.db.Attractions_Reservations_Workers.Find(id);
            return attrResWorker;
        }

        public IList<AttractionReservationWorker> GetAttractionsReservationsWorkers()
        {
            IQueryable<AttractionReservationWorker> attrResWorkers = this.db.Attractions_Reservations_Workers.AsNoTracking();
            return attrResWorkers.ToList();
        }

        public void RemoveAttractionReservationWorker(AttractionReservationWorker attractionReservationWorker)
        {
            this.db.Entry(attractionReservationWorker).State = EntityState.Deleted;
        }

        public void SaveChanges()
        {
            this.db.SaveChanges();
        }

    }
}