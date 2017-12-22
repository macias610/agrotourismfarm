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
using Service.IService;
using DomainModel.Models;

namespace AgrotouristicWebApplication.Controllers
{
    public class HouseTypesController : Controller
    {
        private readonly IHouseTypeService houseTypeService;

        public HouseTypesController(IHouseTypeService houseTypeService)
        {
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
            IList<HouseType> housesTypes = houseTypeService.GetHouseTypes();
            int currentPage = page ?? 1;
            int perPage = 4;
            return View(housesTypes.ToPagedList<HouseType>(currentPage,perPage));
        }

        [Authorize(Roles ="Admin")]
        public ActionResult Create()
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            return View();
        }

        [Authorize(Roles ="Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Type,Price")] HouseType houseType)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            if (ModelState.IsValid)
            {
                try
                {
                    houseTypeService.AddHouseType(houseType);
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
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HouseType houseType = houseTypeService.GetHouseTypeById((int)id);
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
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            string[] fieldsToBind = new string[] { "Type", "Price", "RowVersion" };
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HouseType houseTypeToUpdate = houseTypeService.GetHouseTypeById((int)id);
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
                    houseTypeService.UpdateHouseType(houseTypeToUpdate, rowVersion);
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
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HouseType houseType = houseTypeService.GetHouseTypeById((int)id);
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
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            if (houseTypeService.countHousesOfType(houseType.Id) >= 1)
            {
                ViewBag.error = true;
                return View(houseType);
            }

            try
            {
                houseTypeService.RemoveHouseType(houseType);
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
