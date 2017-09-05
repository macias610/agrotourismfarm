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

        public void AddHouseType(HouseType houseType)
        {
            db.HouseTypes.Add(houseType);
        }

        public int countHousesWithGivenType(int id)
        {
            int numberHouses = (from house in db.Houses
                                where house.HouseTypeId.Equals(id)
                                select house.Id).ToList().Count;
            return numberHouses;
        }

        public List<SelectListItem> getAvaiableTypes()
        {
            List<HouseType> houseTypes = db.HouseTypes.AsNoTracking().ToList();
            List<string> avaiableTypes = new List<string>();
            houseTypes.ForEach(item => avaiableTypes.Add(item.Type));
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

        public HouseType GetHouseTypeById(int id)
        {
            HouseType houseType = db.HouseTypes.Find(id);
            return houseType;
        }

        public HouseType GetHouseTypeByType(string type)
        {
            HouseType houseType = (from item in db.HouseTypes
                                   where item.Type.Equals(type)
                                   select item).FirstOrDefault();
            return houseType;
        }

        public IQueryable<HouseType> GetHouseTypes()
        {
            IQueryable<HouseType> houseTypes = db.HouseTypes.AsNoTracking();
            return houseTypes;
        }

        public void RemoveHouse(House house)
        {
            db.Houses.Remove(house);
        }

        public void RemoveHouseType(HouseType houseType)
        {
            db.HouseTypes.Remove(houseType);
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }

        public void setAvailabilityHouse(House house)
        {
            List<int> reservationsIdForHouse = (from reservation in db.Reservation_Houses
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

        public void UpdateHouse(House house)
        {
            db.Entry(house).State = EntityState.Modified;
        }

        public void UpdateHouseType(HouseType houseType)
        {
            db.Entry(houseType).State = EntityState.Modified;
        }
    }
}