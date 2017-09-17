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
using PagedList;

namespace AgrotouristicWebApplication.Controllers
{
    public class MealsController : Controller
    {
        private readonly IMealRepository repository;

        public MealsController(IMealRepository repository)
        {
            this.repository = repository;
        }

        public ActionResult Index(int? page)
        {
            List<Meal> meals = repository.GetMeals().ToList();
            int currentPage = page ?? 1;
            int perPage = 4;
            return View(meals.ToPagedList<Meal>(currentPage,perPage));
        }

        [Authorize(Roles ="Admin")]
        public ActionResult Create()
        {
            return View();
        }

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
