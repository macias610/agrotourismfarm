using Repository.Models;
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
        IQueryable<HouseType> GetHouseTypes();
        House GetHouseById(int id);
        HouseType GetHouseTypeById(int id);
        HouseType GetHouseTypeByType(string type);
        void setAvailabilityHouse(House house);
        int countHousesWithGivenType(int id);
        List<SelectListItem> getAvaiableTypes();
        void AddHouse(House house);
        void AddHouseType(HouseType houseType);
        void RemoveHouse(House house);
        void RemoveHouseType(HouseType houseType);
        void UpdateHouse(House house);
        void UpdateHouseType(HouseType houseType,byte[] rowVersion);
        void SaveChanges();

    }
}
