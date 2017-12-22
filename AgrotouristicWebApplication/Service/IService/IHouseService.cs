using DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Service.IService
{
    public interface IHouseService
    {
        IList<House> GetHouses();
        House GetHouseById(int id);
        void setAvailabilityHouse(House house);
        void AddHouse(House house);
        void RemoveHouse(House house);
        void UpdateHouse(House house, byte[] rowVersion);
    }
}
