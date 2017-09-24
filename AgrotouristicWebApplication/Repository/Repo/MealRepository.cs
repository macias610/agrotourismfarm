﻿using Repository.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Repository.Models;
using System.Data.Entity;
using System.Web.Mvc;

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

        public int countHousesWithGivenMeal(int id)
        {
            int houses = (from house in db.Reservation_Houses
                                where house.MealId.Equals(id)
                                select house.Id).ToList().Count;
            return houses;
        }

        public Meal GetMealById(int id)
        {
            Meal meal = db.Meals.Find(id);
            return meal;
        }

        public IQueryable<Meal> GetMeals()
        {
            IQueryable<Meal> meals = db.Meals.AsNoTracking();
            return meals;
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