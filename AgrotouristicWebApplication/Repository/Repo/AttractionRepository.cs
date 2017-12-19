using Repository.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Repository.Models;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using DomainModel.Models;

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

        public Attraction GetAttractionById(int id)
        {
            Attraction attraction = db.Attractions.Find(id);
            return attraction;
        }

        public IList<Attraction> GetAttractions()
        {
            IQueryable<Attraction> attractions = db.Attractions.AsNoTracking();
            return attractions.ToList();

        }

        public void RemoveAttraction(Attraction attraction)
        {
            db.Entry(attraction).State = EntityState.Deleted;
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }

        public void UpdateAttraction(Attraction attraction,byte[] rowVersion)
        {
            db.Entry(attraction).OriginalValues["RowVersion"] = rowVersion;
        }
    }
}