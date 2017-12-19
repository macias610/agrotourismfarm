using DomainModel.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepo
{
    public interface IAttractionRepository
    {
        IList<Attraction> GetAttractions();
        Attraction GetAttractionById(int id);
        void AddAttraction(Attraction attraction);
        void UpdateAttraction(Attraction attraction,byte[] rowVersion);
        void RemoveAttraction(Attraction attraction);
        void SaveChanges();
    }
}
