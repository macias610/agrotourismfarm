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
        private readonly IAttractionReservationRepository attractionReservationRepository;

        public AttractionService(IAttractionRepository attractionRepository, IAttractionReservationRepository attractionReservationRepository)
        {
            this.attractionRepository = attractionRepository;
            this.attractionReservationRepository = attractionReservationRepository;
        }

        public void AddAttraction(Attraction attraction)
        {
            this.attractionRepository.AddAttraction(attraction);
            this.attractionRepository.SaveChanges();
        }

        public int countReservationsOfAttraction(int id)
        {
            IList<AttractionReservation> reservations = this.attractionReservationRepository
                                                            .GetAttractionsReservations();
            int quantity = reservations.Where(item => item.AttractionId.Equals(id)).Count();
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
