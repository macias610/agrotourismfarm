using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.IRepo;
using DomainModel.Models;

namespace Service.Service
{
    public class MealService : IMealService
    {
        private readonly IMealRepository mealRepository = null;

        public MealService(IMealRepository mealRepository)
        {
            this.mealRepository = mealRepository;
        }

        public void AddMeal(Meal meal)
        {
            this.mealRepository.AddMeal(meal);
            this.mealRepository.SaveChanges();
        }

        public int countHousesWithGivenMeal(int id)
        {
            int housesQuantity = this.mealRepository.countHousesWithGivenMeal(id);
            return housesQuantity;
        }

        public Meal GetMealById(int id)
        {
            Meal meal = this.mealRepository.GetMealById(id);
            return meal;
        }

        public IList<Meal> GetMeals()
        {
            IList<Meal> meals = this.mealRepository.GetMeals();
            return meals;
        }

        public void RemoveMeal(Meal meal)
        {
            this.mealRepository.RemoveMeal(meal);
            this.mealRepository.SaveChanges();
        }

        public void UpdateMeal(Meal meal, byte[] rowVersion)
        {
            this.mealRepository.UpdateMeal(meal, rowVersion);
            this.mealRepository.SaveChanges();
        }
    }
}
