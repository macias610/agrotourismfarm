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
    public class HousesController : Controller
    {
        private readonly IHouseRepository repository;

        public HousesController(IHouseRepository repository)
        {
            this.repository = repository;
        }

        [Authorize(Roles ="Admin")]
        public ActionResult Index(int? page)
        {
            List<House> houses = repository.GetHouses().ToList();
            houses.ForEach(item => repository.setAvailabilityHouse(item));
 
            List<HouseDetails> houseDetails = new List<HouseDetails>();
            houses.ForEach(item => houseDetails.Add(new HouseDetails()
            {
                House = item,
                Price = repository.GetHouseTypeById(item.HouseTypeId).Price,
                Type = repository.GetHouseTypeById(item.HouseTypeId).Type
            }));
            int currentPage = page ?? 1;
            int perPage = 4;
            return View(houseDetails.ToPagedList<HouseDetails>(currentPage,perPage));
        }

        public ActionResult Create()
        {
            HouseDetails house = new HouseDetails()
            {
                Types = repository.getAvaiableTypes()
            };
            return View(house);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Admin")]
        public ActionResult Create([Bind(Include = "Description,Name")] House house,string selectedTypeText)
        {
            house.HouseTypeId = repository.GetHouseTypeByType(selectedTypeText).Id;
            if (ModelState.IsValid)
            {
                try
                {
                    repository.AddHouse(house);
                    repository.SaveChanges();
                    ViewBag.exception = false;
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
            House house = repository.GetHouseById((int)id);
            if (house == null)
            {
                return HttpNotFound();
            }
            return View(house);
        }

        [Authorize(Roles ="Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Description,Name,HouseTypeId")] House house)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    repository.UpdateHouse(house);
                    repository.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch
                {
                    ViewBag.exception = true;
                    return View(house);
                }
                
            }
            return View(house);
        }

        [Authorize(Roles ="Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            House house = repository.GetHouseById((int)id);
            if (house == null)
            {
                return HttpNotFound();
            }
            return View(house);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            House house = repository.GetHouseById(id);
            repository.setAvailabilityHouse(house);

            if (house.statusHouse.Equals("Zajęty")|| house.statusHouse.Equals("Zarezerwowany"))
            {
                ViewBag.error = true;
                return View(house);
            }


            try
            {
                repository.RemoveHouse(house);
                repository.SaveChanges();
            }
            catch
            {
                ViewBag.exception = true;
                return View(house);
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
