using DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IAttractionService
    {
        IList<Attraction> GetAttractions();
        Attraction GetAttractionById(int id);
        int countReservationsOfAttraction(int id);
        void AddAttraction(Attraction attraction);
        void UpdateAttraction(Attraction attraction, byte[] rowVersion);
        void RemoveAttraction(Attraction attraction);
    }
}
