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
    public class AttractionsController : Controller
    {
        private readonly IAttractionRepository repository;

        public AttractionsController(IAttractionRepository repository)
        {
            this.repository = repository;
        }

        [Authorize(Roles ="Admin")]
        public ActionResult Index(int? page)
        {
            List<Attraction> attractions = repository.GetAttractions().ToList();
            int currentPage = page ?? 1;
            int perPage = 4;
            return View(attractions.ToPagedList<Attraction>(currentPage,perPage));
        }

        [Authorize(Roles ="Admin")]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles ="Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,Price,Discount")] Attraction attraction)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    repository.AddAttraction(attraction);
                    repository.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch
                {
                    ViewBag.exception = true;
                    return View();
                }
            }

            return View();
        }

        [Authorize(Roles ="Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Attraction attraction = repository.GetAttractionById((int)id);
            if (attraction == null)
            {
                return HttpNotFound();
            }
            return View(attraction);
        }

        [Authorize(Roles ="Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id,byte[] rowVersion)
        {
            string[] fieldsToBind = new string[] { "Name", "Description", "Price", "Discount", "RowVersion" };
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Attraction attractionToUpdate = repository.GetAttractionById((int)id);
            if (attractionToUpdate == null)
            {
                Attraction deletedAttraction = new Attraction();
                TryUpdateModel(deletedAttraction, fieldsToBind);
                ModelState.AddModelError(string.Empty,
                    "Nie można zapisać. Warsztat został usunięty przez innego użytkownika.");
                return View(deletedAttraction);
            }
            if (TryUpdateModel(attractionToUpdate, fieldsToBind))
            {
                try
                {
                    repository.UpdateAttraction(attractionToUpdate, rowVersion);
                    repository.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    DbEntityEntry entry = ex.Entries.Single();
                    Attraction clientValues = (Attraction)entry.Entity;
                    DbPropertyValues databaseEntry = entry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty,
                            "Nie można zapisać. Warsztat został usunięty przez innego użytkownika.");
                    }
                    else
                    {
                        Attraction databaseValues = (Attraction)databaseEntry.ToObject();

                        if (databaseValues.Name != clientValues.Name)
                            ModelState.AddModelError("Nazwa", "Aktulalna wartość: "
                                + databaseValues.Name);
                        if (databaseValues.Description != clientValues.Description)
                            ModelState.AddModelError("Opis", "Aktulalna wartość: "
                                + String.Format("{0:c}", databaseValues.Description));
                        if (databaseValues.Price != clientValues.Price)
                            ModelState.AddModelError("Cena", "Aktualna wartość: "
                                + String.Format("{0:d}", databaseValues.Price));
                        if (databaseValues.Discount != clientValues.Discount)
                            ModelState.AddModelError("Zniżka", "Aktualna wartość: "
                                + String.Format("{0:d}", clientValues.Discount));
                        ModelState.AddModelError(string.Empty, "Edytowany rekord  "
                            + "został wcześniej zmodyfikowany przez innego użytkownika,operacja anulowana. Pobrano wpisane wartości. Kliknij zapisz, żeby napisać.");
                        attractionToUpdate.RowVersion = databaseValues.RowVersion;
                    }
                }
                catch (RetryLimitExceededException )
                {
                    ModelState.AddModelError("", "Nie można zapisać. Spróbuj ponownie");
                }
            }

            return View(attractionToUpdate);
        }

        [Authorize(Roles ="Admin")]
        public ActionResult Delete(int? id,bool? concurrencyError)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Attraction attraction = repository.GetAttractionById((int)id);
            if (attraction == null)
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
            return View(attraction);
        }

        [Authorize(Roles ="Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed([Bind(Include ="Id,Name,Descripction,Price,Discount,RowVersion")] Attraction attraction)
        {

            if (repository.countReservationsWithGivenAttraction(attraction.Id) >= 1)
            {
                ViewBag.error = true;
                return View(attraction);
            }
            try
            {
                repository.RemoveAttraction(attraction);
                repository.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction("Delete", new { concurrencyError = true, id = attraction.Id });
            }
            catch (DataException )
            {
                ModelState.AddModelError(string.Empty, "Nie można usunąć.Spróbuj ponownie.");
                return View(attraction);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
