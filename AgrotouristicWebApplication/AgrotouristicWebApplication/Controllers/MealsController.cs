using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Repository.Models;
using Repository.IRepo;

namespace AgrotouristicWebApplication.Controllers
{
    public class MealsController : Controller
    {
        private readonly IMealRepository repository;

        public MealsController(IMealRepository repository)
        {
            this.repository = repository;
        }

        // GET: Meals
        public ActionResult Index()
        {
            List<Meal> meals = repository.GetMeals().ToList();
            return View(meals);
        }

        // GET: Meals/Create
        [Authorize(Roles ="Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Meals/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles ="Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Type,Price")] Meal meal)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    repository.AddMeal(meal);
                    repository.SaveChanges();
                    ViewBag.exception = false;
                    return RedirectToAction("Index");
                }
                catch(Exception e)
                {
                    ViewBag.exception = true;
                    return View();
                }
            }
            return View();
        }

        // GET: Meals/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Meal meal = repository.GetMealById((int)id);
            if (meal == null)
            {
                return HttpNotFound();
            }
            return View(meal);
        }

        // POST: Meals/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Type,Price")] Meal meal)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    repository.UpdateMeal(meal);
                    repository.SaveChanges();
                    ViewBag.exception = false;
                    return RedirectToAction("Index");
                }
                catch
                {
                    ViewBag.exception = true;
                    return View(meal);
                }              
               
            }
            return View(meal);
        }

        // GET: Meals/Delete/5
        [Authorize(Roles ="Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Meal meal = repository.GetMealById((int)id);
            if (meal == null)
            {
                return HttpNotFound();
            }
            return View(meal);
        }

        // POST: Meals/Delete/5
        [Authorize(Roles ="Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Meal meal = repository.GetMealById(id);

            if (repository.countHousesWithGivenMeal(id) >= 1)
            {
                ViewBag.error = true;
                return View(meal);
            }

            try
            {
                repository.RemoveMeal(meal);
                repository.SaveChanges();
            }
            catch
            {
                ViewBag.exception = true;
                return View(meal);
            }
            ViewBag.error = false;
            ViewBag.exception = false;
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
