using DomainModel.Models;
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
        IList<House> GetHouses();
        IList<HouseType> GetHouseTypes();
        House GetHouseById(int id);
        HouseType GetHouseTypeById(int id);
        HouseType GetHouseTypeByType(string type);
        void setAvailabilityHouse(House house);
        int countHousesWithGivenType(int id);
        IList<SelectListItem> getAvaiableTypes();
        void AddHouse(House house);
        void AddHouseType(HouseType houseType);
        void RemoveHouse(House house);
        void RemoveHouseType(HouseType houseType);
        void UpdateHouse(House house,byte[] rowVersion);
        void UpdateHouseType(HouseType houseType,byte[] rowVersion);
        void SaveChanges();

    }
}
