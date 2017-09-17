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
        public ActionResult Edit([Bind(Include = "Id,Type,Price")] HouseType houseType)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    repository.UpdateHouseType(houseType);
                    repository.SaveChanges();
                    
                }
                catch
                {
                    ViewBag.exception = true;
                    return View();
                }

            }
            return RedirectToAction("Index");
        }

        // GET: HouseTypes/Delete/5
        [Authorize(Roles ="Admin")]
        public ActionResult Delete(int? id)
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

        // POST: HouseTypes/Delete/5
        [Authorize(Roles ="Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            HouseType houseType = repository.GetHouseTypeById(id);
            if (repository.countHousesWithGivenType(id) >= 1)
            {
                ViewBag.error = true;
                return View(houseType);
            }

            try
            {
                repository.RemoveHouseType(houseType);
                repository.SaveChanges();   
            }
            catch
            {
                ViewBag.excepton = true;
                return View(houseType);
            }
            ViewBag.exception = false;
            ViewBag.error = false;
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
