using DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Repository.IRepo
{
    public interface IMealRepository
    {
        IList<Meal> GetMeals();
        Meal GetMealById(int id);
        Meal GetMealByType(string type);
        void AddMeal(Meal meal);
        void UpdateMeal(Meal meal,byte[] rowVersion);
        void RemoveMeal(Meal meal);
        void SaveChanges();
    }
}
