using Repository.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Repository.Models;
using System.Web.Mvc;
using System.Data.Entity;
using DomainModel.Models;

namespace Repository.Repo
{
    public class HouseRepository : IHouseRepository
    {

        private readonly IAgrotourismContext db;

        public HouseRepository(IAgrotourismContext db)
        {
            this.db = db;
        }

        public void AddHouse(House house)
        {
            db.Houses.Add(house);
        }


        public House GetHouseById(int id)
        {
            House house = db.Houses.Find(id);
            return house;
        }

        public IList<House> GetHouses()
        {
            IQueryable<House> houses = db.Houses.AsNoTracking();
            return houses.ToList();
        }

        public void RemoveHouse(House house)
        {
            db.Entry(house).State=EntityState.Deleted;
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }

        public void UpdateHouse(House house,byte[] rowVersion)
        {
            db.Entry(house).OriginalValues["RowVersion"] = rowVersion;
        }
    }
}