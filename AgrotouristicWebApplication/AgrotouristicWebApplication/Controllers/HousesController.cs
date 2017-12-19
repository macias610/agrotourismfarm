using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Data.Entity.Infrastructure;
using Microsoft.AspNet.Identity;
using DomainModel.Models;
using Service.IService;
using ViewModel;

namespace AgrotouristicWebApplication.Controllers
{
    public class HousesController : Controller
    {
        private readonly IHouseService houseService;
        private readonly IHouseTypeService houseTypeService;

        public HousesController(IHouseService houseService, IHouseTypeService houseTypeService)
        {
            this.houseService = houseService;
            this.houseTypeService = houseTypeService;
        }

        [Authorize(Roles ="Admin")]
        public ActionResult Index(int? page)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            List<House> houses = houseService.GetHouses().ToList();
            houses.ForEach(item => houseService.setAvailabilityHouse(item));
 
            List<HouseDetails> houseDetails = new List<HouseDetails>();
            houses.ForEach(item => houseDetails.Add(new HouseDetails()
            {
                House = item,
                Price = houseTypeService.GetHouseTypeById(item.HouseTypeId).Price,
                Type = houseTypeService.GetHouseTypeById(item.HouseTypeId).Type
            }));
            int currentPage = page ?? 1;
            int perPage = 4;
            return View(houseDetails.ToPagedList<HouseDetails>(currentPage,perPage));
        }

        public ActionResult Create()
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            HouseDetails house = new HouseDetails()
            {
                Types = houseTypeService.getHouseTypesAsSelectList()
            };
            return View(house);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Admin")]
        public ActionResult Create([Bind(Include = "Description,Name")] House house,string selectedTypeText)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            house.HouseTypeId = houseTypeService.GetHouseTypeByType(selectedTypeText).Id;
            if (ModelState.IsValid)
            {
                try
                {
                    houseService.AddHouse(house);
                    ViewBag.exception = false;
                    return RedirectToAction("Index");

                }
                catch 
                {
                    ViewBag.exception = true;
                    HouseDetails exc = new HouseDetails()
                    {
                        Types = houseTypeService.getHouseTypesAsSelectList()
                    };
                    return View(exc);
                }

            }
            HouseDetails again = new HouseDetails()
            {
                Types = houseTypeService.getHouseTypesAsSelectList()
            };
            return View(again);
        }

        [Authorize(Roles ="Admin")]
        public ActionResult Edit(int? id)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            House house = houseService.GetHouseById((int)id);
            if (house == null)
            {
                return HttpNotFound();
            }
            return View(house);
        }

        [Authorize(Roles ="Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, byte[] rowVersion) 
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            string[] fieldsToBind = new string[] { "Description", "Name","HouseTypeId", "RowVersion" };
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            House houseToUpdate = houseService.GetHouseById((int)id);
            if (houseToUpdate == null)
            {
                House deletedHouse = new House();
                TryUpdateModel(deletedHouse, fieldsToBind);
                 ModelState.AddModelError(string.Empty,
                    "Nie można zapisać. Domek został usunięty przez innego użytkownika.");
                return View(deletedHouse);
            }
            if (TryUpdateModel(houseToUpdate, fieldsToBind))
            {
                try
                {
                    houseService.UpdateHouse(houseToUpdate, rowVersion);
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    DbEntityEntry entry = ex.Entries.Single();
                    House clientValues = (House)entry.Entity;
                    DbPropertyValues databaseEntry = entry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty,
                            "Nie można zapisać. Domek został usunięty przez innego użytkownika.");
                    }
                    else
                    {
                        House databaseValues = (House)databaseEntry.ToObject();

                        if (databaseValues.Name != clientValues.Name)
                            ModelState.AddModelError("Nazwa", "Aktulalna wartość: "
                                + databaseValues.Name);
                        if (databaseValues.HouseTypeId != clientValues.HouseTypeId)
                            ModelState.AddModelError("Typdomku", "Aktulalna wartość: "
                                + String.Format("{0:c}", databaseValues.HouseTypeId));
                        if (databaseValues.Description != clientValues.Description)
                            ModelState.AddModelError("Description", "Aktulalna wartość: "
                                + String.Format("{0:c}", databaseValues.Description));
                        ModelState.AddModelError(string.Empty, "Edytowany rekord  "
                            + "został wcześniej zmodyfikowany przez innego użytkownika,operacja anulowana. Pobrano wpisane wartości. Kliknij zapisz, żeby napisać.");
                        houseToUpdate.RowVersion = databaseValues.RowVersion;
                    }
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Nie można zapisać. Spróbuj ponownie");
                }
            }

            return View(houseToUpdate);
        }

        [Authorize(Roles ="Admin")]
        public ActionResult Delete(int? id, bool? concurrencyError)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            House house = houseService.GetHouseById((int)id);
            if (house == null)
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
            return View(house);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Admin")]
        public ActionResult DeleteConfirmed([Bind(Include ="Id,Description,Name,RowVersion")] House house)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            houseService.setAvailabilityHouse(house);

            if (house.statusHouse.Equals("Zajęty")|| house.statusHouse.Equals("Zarezerwowany"))
            {
                ViewBag.error = true;
                return View(house);
            }
            try
            {
                houseService.RemoveHouse(house);
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction("Delete", new { concurrencyError = true, id = house.Id });
            }
            catch (DataException)
            {
                ModelState.AddModelError(string.Empty, "Nie można usunąć.Spróbuj ponownie.");
                return View(house);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
