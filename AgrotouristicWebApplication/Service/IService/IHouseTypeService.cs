using DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Service.IService
{
    public interface IHouseTypeService
    {
        IList<HouseType> GetHouseTypes();
        HouseType GetHouseTypeById(int id);
        HouseType GetHouseTypeByType(string type);
        IList<SelectListItem> getHouseTypesAsSelectList();
        int countHousesOfType(int id);
        void AddHouseType(HouseType houseType);
        void RemoveHouseType(HouseType houseType);
        void UpdateHouseType(HouseType houseType, byte[] rowVersion);

    }
}
