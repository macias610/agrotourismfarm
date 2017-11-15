using DomainModel.Models;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IMealService
    {
        IList<Meal> GetMeals();
        Meal GetMealById(int id);
        void AddMeal(Meal meal);
        void UpdateMeal(Meal meal, byte[] rowVersion);
        int countHousesWithGivenMeal(int id);
        void RemoveMeal(Meal meal);
    }
}
