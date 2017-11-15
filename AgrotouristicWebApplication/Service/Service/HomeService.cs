using Repository.IRepo;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class HomeService : IHomeService
    {
        private readonly IHomeRepository homeRepository = null;

        public HomeService(IHomeRepository homeRepository)
        {
            this.homeRepository = homeRepository;
        }

        public IList<string> GetAvaiableAttractions()
        {
            IList<string> attractions = this.homeRepository.GetAvaiableAttractions();
            return attractions;
        }

        public IList<string> GetAvaiableHouses()
        {
            IList<string> houses = this.homeRepository.GetAvaiableHouses();
            return houses;
        }

        public IList<string> GetAvaiableMeals()
        {
            IList<string> meals = this.homeRepository.GetAvaiableMeals();
            return meals;
        }
    }
}
