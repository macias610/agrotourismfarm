using Repository.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repository.Repo
{
    public class HomeRepository : IHomeRepository
    {
        private readonly IAgrotourismContext db;

        public HomeRepository(IAgrotourismContext db)
        {
            this.db = db;
        }

        public List<string> GetAvaiableAttractions()
        {
            List<string> attractions = (from attraction in db.Attractions
                                        select attraction.Name).ToList();
            return attractions;
        }

        public List<string> GetAvaiableHouses()
        {
            List<string> houses = (from house in db.Houses
                                   select house.HouseType.Type + "(" + house.Name + ")").ToList();
            return houses;
        } 

        public List<string> GetAvaiableMeals()
        {
            List<string> meals = (from meal in db.Meals
                                  select meal.Type).ToList();
            return meals;
        }
    }
}