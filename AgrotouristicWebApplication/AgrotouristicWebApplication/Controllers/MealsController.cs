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
using System.Data.Entity.Infrastructure;

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
        public ActionResult Edit(int? id, byte[] rowVersion)
        {
            string[] fieldsToBind = new string[] { "Type", "Price", "RowVersion" };
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Meal mealToUpdate = repository.GetMealById((int)id);
            if (mealToUpdate == null)
            {
                Meal deletedMeal = new Meal();
                TryUpdateModel(deletedMeal, fieldsToBind);
                ModelState.AddModelError(string.Empty,
                    "Nie można zapisać. Posiłek został usunięty przez innego użytkownika.");
                return View(deletedMeal);
            }
            if (TryUpdateModel(mealToUpdate, fieldsToBind))
            {
                try
                {
                    repository.UpdateMeal(mealToUpdate, rowVersion);
                    repository.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    DbEntityEntry entry = ex.Entries.Single();
                    Meal clientValues = (Meal)entry.Entity;
                    DbPropertyValues databaseEntry = entry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty,
                            "Nie można zapisać. Posiłek został usunięty przez innego użytkownika.");
                    }
                    else
                    {
                        Meal databaseValues = (Meal)databaseEntry.ToObject();

                        if (databaseValues.Type != clientValues.Type)
                            ModelState.AddModelError("Typ", "Aktulalna wartość: "
                                + databaseValues.Type);
                        if (databaseValues.Price != clientValues.Price)
                            ModelState.AddModelError("Cena", "Aktulalna wartość: "
                                + String.Format("{0:c}", databaseValues.Price));
                        ModelState.AddModelError(string.Empty, "Edytowany rekord  "
                            + "został wcześniej zmodyfikowany przez innego użytkownika,operacja anulowana. Pobrano wpisane wartości. Kliknij zapisz, żeby napisać.");
                        mealToUpdate.RowVersion = databaseValues.RowVersion;
                    }
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Nie można zapisać. Spróbuj ponownie");
                }
            }

            return View(mealToUpdate);
        }

        [Authorize(Roles ="Admin")]
        public ActionResult Delete(int? id,bool? concurrencyError)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Meal meal = repository.GetMealById((int)id);
            if (meal == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("Index");
                }
                return HttpNotFound();
            }
            if (concurrencyError.GetValueOrDefault())
            {
                ViewBag.ConcurrencyErrorMessage = "Usuwany rekord "
                    + "został zmodyfikowany po pobraniu oryginalnych wartości."
                    + "Usuwanie anulowano i wyświetlono aktualne dane. "
                    + "W celu usunięcia kliknij usuń";
            }
            return View(meal);
        }

        [Authorize(Roles ="Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed([Bind(Include ="Id,Price,Type,RowVersion")]Meal meal)
        {

            if (repository.countHousesWithGivenMeal(meal.Id) >= 1)
            {
                ViewBag.error = true;
                return View(meal);
            }
            try
            {
                repository.RemoveMeal(meal);
                repository.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction("Delete", new { concurrencyError = true, id = meal.Id });
            }
            catch (DataException)
            {
                ModelState.AddModelError(string.Empty, "Nie można usunąć.Spróbuj ponownie.");
                return View(meal);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
