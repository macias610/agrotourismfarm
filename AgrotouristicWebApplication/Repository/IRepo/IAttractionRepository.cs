using Microsoft.AspNet.Identity.EntityFramework;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepo
{
    public interface IAttractionRepository
    {
        IQueryable<Attraction> GetAttractions();
        Attraction GetAttractionById(int id);
        int countReservationsWithGivenAttraction(int id);
        void AddAttraction(Attraction attraction);
        void UpdateAttraction(Attraction attraction,byte[] rowVersion);
        void RemoveAttraction(Attraction attraction);
        void SaveChanges();
    }
}
