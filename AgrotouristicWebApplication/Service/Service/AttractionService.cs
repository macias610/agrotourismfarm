using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainModel.Models;
using Repository.IRepo;

namespace Service.Service
{
    public class AttractionService : IAttractionService
    {
        private readonly IAttractionRepository attractionRepository;

        public AttractionService(IAttractionRepository attractionRepository)
        {
            this.attractionRepository = attractionRepository;
        }

        public void AddAttraction(Attraction attraction)
        {
            this.attractionRepository.AddAttraction(attraction);
            this.attractionRepository.SaveChanges();
        }

        public int countReservationsWithGivenAttraction(int id)
        {
            int quantity = this.attractionRepository.countReservationsWithGivenAttraction(id);
            return quantity;
        }

        public Attraction GetAttractionById(int id)
        {
            Attraction attraction = this.attractionRepository.GetAttractionById(id);
            return attraction;
        }

        public IList<Attraction> GetAttractions()
        {
            IList<Attraction> attractions = this.attractionRepository.GetAttractions();
            return attractions;
        }

        public void RemoveAttraction(Attraction attraction)
        {
            this.attractionRepository.RemoveAttraction(attraction);
            this.attractionRepository.SaveChanges();
        }

        public void UpdateAttraction(Attraction attraction, byte[] rowVersion)
        {
            this.attractionRepository.UpdateAttraction(attraction, rowVersion);
            this.attractionRepository.SaveChanges();
        }
    }
}
