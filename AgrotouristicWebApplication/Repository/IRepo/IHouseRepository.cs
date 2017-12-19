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
        House GetHouseById(int id);
        void AddHouse(House house);
        void RemoveHouse(House house);
        void UpdateHouse(House house,byte[] rowVersion);
        void SaveChanges();

    }
}
