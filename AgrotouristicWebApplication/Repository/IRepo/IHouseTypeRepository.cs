using DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Repository.IRepo
{
    public interface IHouseTypeRepository
    {
        IList<HouseType> GetHouseTypes();

        HouseType GetHouseTypeById(int id);

        HouseType GetHouseTypeByType(string type);

        IList<SelectListItem> getAvaiableTypes();

        void AddHouseType(HouseType houseType);

        void RemoveHouseType(HouseType houseType);

        void UpdateHouseType(HouseType houseType, byte[] rowVersion);

        void SaveChanges();


    }
}
