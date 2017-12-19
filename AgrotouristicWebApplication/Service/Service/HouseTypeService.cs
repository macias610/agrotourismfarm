using DomainModel.Models;
using Repository.IRepo;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Service.Service
{
    public class HouseTypeService : IHouseTypeService
    {
        private readonly IHouseTypeRepository houseTypeRepository = null;
        private readonly IHouseRepository houseRepository = null;

        public HouseTypeService(IHouseTypeRepository houseTypeRepository, IHouseRepository houseRepository)
        {
            this.houseTypeRepository = houseTypeRepository;
            this.houseRepository = houseRepository;
        }

        public void AddHouseType(HouseType houseType)
        {
            this.houseTypeRepository.AddHouseType(houseType);
            this.houseTypeRepository.SaveChanges();
        }

        public int countHousesOfType(int id)
        {
            IList<House> houses = this.houseRepository.GetHouses();
            int quantity = houses.Where(item => item.HouseTypeId.Equals(id)).Count();
            return quantity;
        }

        public IList<SelectListItem> getHouseTypesAsSelectList()
        {
            IList<HouseType> houseTypes = this.houseTypeRepository.GetHouseTypes();
            IList<string> avaiableTypes = new List<string>();
            houseTypes.ToList().ForEach(item => avaiableTypes.Add(item.Type));
            IList<SelectListItem> selectList = avaiableTypes.Select(avaiableType => new SelectListItem { Value = avaiableType, Text = avaiableType }).ToList();
            return selectList;
        }

        public HouseType GetHouseTypeById(int id)
        {
            HouseType houseType = this.houseTypeRepository.GetHouseTypeById(id);
            return houseType;
        }

        public HouseType GetHouseTypeByType(string type)
        {
            HouseType houseType = this.houseTypeRepository.GetHouseTypeByType(type);
            return houseType;
        }

        public IList<HouseType> GetHouseTypes()
        {
            IList<HouseType> houseTypes = this.houseTypeRepository.GetHouseTypes();
            return houseTypes;
        }

        public void RemoveHouseType(HouseType houseType)
        {
            this.houseTypeRepository.RemoveHouseType(houseType);
            this.houseTypeRepository.SaveChanges();
        }

        public void UpdateHouseType(HouseType houseType, byte[] rowVersion)
        {
            this.houseTypeRepository.UpdateHouseType(houseType, rowVersion);
            this.houseTypeRepository.SaveChanges();
        }

    }
}
