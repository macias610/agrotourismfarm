using Repository.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using DomainModel.Models;

namespace Repository.Repo
{
    public class MealRepository : IMealRepository
    {
        private readonly IAgrotourismContext db;

        public MealRepository(IAgrotourismContext db)
        {
            this.db = db;
        }

        public void AddMeal(Meal meal)
        {
            db.Meals.Add(meal);
        }

        public Meal GetMealById(int id)
        {
            Meal meal = db.Meals.Find(id);
            return meal;
        }

        public Meal GetMealByType(string type)
        {
            Meal meal = this.db.Meals.Where(item => item.Type.Equals(type)).SingleOrDefault();
            return meal;
        }

        public IList<Meal> GetMeals()
        {
            IQueryable<Meal> meals = db.Meals.AsNoTracking();
            return meals.ToList() ;
        }

        public void RemoveMeal(Meal meal)
        {
            db.Entry(meal).State = EntityState.Deleted;
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }

        public void UpdateMeal(Meal meal,byte[] rowVersion)
        {
            db.Entry(meal).OriginalValues["RowVersion"] = rowVersion;
        }
    }
}