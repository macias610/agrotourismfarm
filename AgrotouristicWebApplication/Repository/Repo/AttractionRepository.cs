using Repository.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Repository.Models;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Repository.Repo
{
    public class AttractionRepository: IAttractionRepository
    {
        private readonly IAgrotourismContext db;

        public AttractionRepository(IAgrotourismContext db)
        {
            this.db = db;
        }

        public void AddAttraction(Attraction attraction)
        {
            db.Attractions.Add(attraction);
        }

        public int countReservationsWithGivenAttraction(int id)
        {
            int reservations = (from reservation in db.Attractions_Reservations
                                where reservation.AttractionId.Equals(id)
                                select reservation.Id).ToList().Count;
            return reservations;
        }

        public Attraction GetAttractionById(int id)
        {
            Attraction attraction = db.Attractions.Find(id);
            return attraction;
        }

        public IQueryable<Attraction> GetAttractions()
        {
            IQueryable<Attraction> attractions = db.Attractions.AsNoTracking();
            return attractions;

        }

        public void RemoveAttraction(Attraction attraction)
        {
            db.Attractions.Remove(attraction);
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }

        public void UpdateAttraction(Attraction attraction)
        {
            db.Entry(attraction).State = EntityState.Modified;
        }
    }
}