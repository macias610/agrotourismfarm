﻿using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Repository.IRepo
{
    public interface IHouseRepository
    {
        IQueryable<House> GetHouses();
        House GetHouseById(int id);
        IQueryable<House> GetHousesByType(string type);
        void setAvailabilityHouse(House house);
        List<SelectListItem> getAvaiableTypes();
        void setPriceCreatedHouse(House house);
        void AddHouse(House house);
        void RemoveHouse(House house);
        void UpdateHouse(House house);
        void SaveChanges();

    }
}
