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
using Repository.ViewModels;
using PagedList;
using System.Data.Entity.Infrastructure;

namespace AgrotouristicWebApplication.Controllers
{
    public class HouseTypesController : Controller
    {
        private readonly IHouseRepository repository;

        public HouseTypesController(IHouseRepository repository)
        {
            this.repository = repository;
        }

        // GET: HouseTypes
        [Authorize(Roles ="Admin")]
        public ActionResult Index(int? page)
        {
            List<HouseType> housesTypes = repository.GetHouseTypes().ToList();
            int currentPage = page ?? 1;
            int perPage = 4;
            return View(housesTypes.ToPagedList<HouseType>(currentPage,perPage));
        }

        // GET: HouseTypes/Create
        [Authorize(Roles ="Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: HouseTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles ="Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Type,Price")] HouseType houseType)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    repository.AddHouseType(houseType);
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

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HouseType houseType = repository.GetHouseTypeById((int)id);
            if (houseType == null)
            {
                return HttpNotFound();
            }
            return View(houseType);
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
            HouseType houseTypeToUpdate = repository.GetHouseTypeById((int)id);
            if (houseTypeToUpdate == null)
            {
                HouseType deletedHouseType = new HouseType();
                TryUpdateModel(deletedHouseType, fieldsToBind);
                ModelState.AddModelError(string.Empty,
                    "Nie można zapisać. Typ domku został usunięty przez innego użytkownika.");
                return View(deletedHouseType);
            }
            if (TryUpdateModel(houseTypeToUpdate, fieldsToBind))
            {
                try
                {
                    repository.UpdateHouseType(houseTypeToUpdate, rowVersion);
                    repository.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    DbEntityEntry entry = ex.Entries.Single();
                    HouseType clientValues = (HouseType)entry.Entity;
                    DbPropertyValues databaseEntry = entry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty,
                            "Nie można zapisać. Typ domku został usunięty przez innego użytkownika.");
                    }
                    else
                    {
                        HouseType databaseValues = (HouseType)databaseEntry.ToObject();

                        if (databaseValues.Type != clientValues.Type)
                            ModelState.AddModelError("Typ", "Aktulalna wartość: "
                                + databaseValues.Type);
                        if (databaseValues.Price != clientValues.Price)
                            ModelState.AddModelError("Cena", "Aktulalna wartość: "
                                + String.Format("{0:c}", databaseValues.Price));
                        ModelState.AddModelError(string.Empty, "Edytowany rekord  "
                            + "został wcześniej zmodyfikowany przez innego użytkownika,operacja anulowana. Pobrano wpisane wartości. Kliknij zapisz, żeby napisać.");
                        houseTypeToUpdate.RowVersion = databaseValues.RowVersion;
                    }
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Nie można zapisać. Spróbuj ponownie");
                }
            }

            return View(houseTypeToUpdate);
        }

        [Authorize(Roles ="Admin")]
        public ActionResult Delete(int? id,bool? concurrencyError)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HouseType houseType = repository.GetHouseTypeById((int)id);
            if (houseType == null)
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
            return View(houseType);
        }

        [Authorize(Roles ="Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed([Bind(Include ="Id,Type,Price,RowVersion")] HouseType houseType)
        {
            if (repository.countHousesWithGivenType(houseType.Id) >= 1)
            {
                ViewBag.error = true;
                return View(houseType);
            }

            try
            {
                repository.RemoveHouseType(houseType);
                repository.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction("Delete", new { concurrencyError = true, id = houseType.Id });
            }
            catch (DataException)
            {
                ModelState.AddModelError(string.Empty, "Nie można usunąć.Spróbuj ponownie.");
                return View(houseType);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
