using Repository.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Repository.Models;
using System.Web.Mvc;
using System.Data.Entity;

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

        public List<SelectListItem> getAvaiableTypes()
        {
            List<string> avaiableTypes = new List<string>()
            {
                {"2-osobowy" },
                {"3-osobowy" },
                {"4-osobowy" },
                {"5-osobowy" }
            };

            List<SelectListItem> selectList = avaiableTypes.Select(avaiableType => new SelectListItem { Value = avaiableType, Text = avaiableType }).ToList();
            return selectList;
        }

        public House GetHouseById(int id)
        {
            House house = db.Houses.Find(id);
            return house;
        }

        public IQueryable<House> GetHouses()
        {
            IQueryable<House> houses = db.Houses.AsNoTracking();
            return houses;
        }

        public IQueryable<House> GetHousesByType(string type)
        {
            IQueryable<House> houses = from house in db.Houses
                                       where house.Type.Equals(type)
                                       select house;
            return houses;
        }

        public void RemoveHouse(House house)
        {
            db.Houses.Remove(house);
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }

        public void setAvailabilityHouse(House house)
        {
            List<int> reservationsIdForHouse = (from reservation in db.Reservation_House
                                                where reservation.HouseId.Equals(house.Id)
                                                select reservation.ReservationId).ToList();

            List<Reservation> reservations = (from reservation in db.Reservations
                                              where reservationsIdForHouse.Contains(reservation.Id)
                                              where reservation.StartDate <= DateTime.Now
                                              where reservation.EndDate >= DateTime.Now
                                              select reservation).ToList();
            if (reservations.Count >= 1)
            {
                house.statusHouse = "Zajęty";
            }
            else if(reservationsIdForHouse.Count >= 1)
            {
                house.statusHouse = "Zarezerwowany";
            }
            else
            {
                house.statusHouse = "Wolny";
            }
        }

        public void setPriceCreatedHouse(House house)
        {
            List<House> houses = GetHousesByType(house.Type).ToList();
            if (houses.Count >= 1)
            {
                house.Price = houses.FirstOrDefault().Price;
            }
            else
            {
                house.Price = 100;
            }
        }

        public void UpdateHouse(House house)
        {
            db.Entry(house).State = EntityState.Modified;
        }
    }
}