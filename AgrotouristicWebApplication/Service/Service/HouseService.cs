using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainModel.Models;
using System.Web.Mvc;
using Repository.IRepo;

namespace Service.Service
{
    public class HouseService : IHouseService
    {
        private readonly IHouseRepository houseRepository = null;

        public HouseService(IHouseRepository houseService)
        {
            this.houseRepository = houseService;
        }

        public void AddHouse(House house)
        {
            this.houseRepository.AddHouse(house);
            this.houseRepository.SaveChanges();
        }

        public void AddHouseType(HouseType houseType)
        {
            this.houseRepository.AddHouseType(houseType);
            this.houseRepository.SaveChanges();
        }

        public int countHousesWithGivenType(int id)
        {
            int quantity = this.houseRepository.countHousesWithGivenType(id);
            return quantity;
        }

        public IList<SelectListItem> getAvaiableTypes()
        {
            IList<SelectListItem> avaiableTypes = this.houseRepository.getAvaiableTypes();
            return avaiableTypes;
        }

        public House GetHouseById(int id)
        {
            House house = this.houseRepository.GetHouseById(id);
            return house;
        }

        public IList<House> GetHouses()
        {
            IList<House> houses = this.houseRepository.GetHouses();
            return houses;
        }

        public HouseType GetHouseTypeById(int id)
        {
            HouseType houseType = this.houseRepository.GetHouseTypeById(id);
            return houseType;
        }

        public HouseType GetHouseTypeByType(string type)
        {
            HouseType houseType = this.houseRepository.GetHouseTypeByType(type);
            return houseType;
        }

        public IList<HouseType> GetHouseTypes()
        {
            IList<HouseType> houseTypes = this.houseRepository.GetHouseTypes();
            return houseTypes;
        }

        public void RemoveHouse(House house)
        {
            this.houseRepository.RemoveHouse(house);
            this.houseRepository.SaveChanges();

        }

        public void RemoveHouseType(HouseType houseType)
        {
            this.houseRepository.RemoveHouseType(houseType);
            this.houseRepository.SaveChanges();
        }

        public void setAvailabilityHouse(House house)
        {
            this.houseRepository.setAvailabilityHouse(house);
        }

        public void UpdateHouse(House house, byte[] rowVersion)
        {
            this.houseRepository.UpdateHouse(house, rowVersion);
            this.houseRepository.SaveChanges();
        }

        public void UpdateHouseType(HouseType houseType, byte[] rowVersion)
        {
            this.houseRepository.UpdateHouseType(houseType, rowVersion);
            this.houseRepository.SaveChanges();
        }
    }
}
