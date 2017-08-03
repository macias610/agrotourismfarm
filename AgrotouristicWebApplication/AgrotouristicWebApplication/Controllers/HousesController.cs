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

namespace AgrotouristicWebApplication.Controllers
{
    public class HousesController : Controller
    {
        private readonly IHouseRepository repository;

        public HousesController(IHouseRepository repository)
        {
            this.repository = repository;
        }

        // GET: Houses
        [Authorize(Roles ="Admin")]
        public ActionResult Index()
        {
            List<House> houses = repository.GetHouses().ToList();

            houses.ForEach(item => repository.setAvailabilityHouse(item));

            return View(houses);
        }

        // GET: Houses/Create
        public ActionResult Create()
        {
            NewHouse house = new NewHouse()
            {
                Types = repository.getAvaiableTypes()
            };
            return View(house);
        }

        //// POST: Houses/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Admin")]
        public ActionResult Create([Bind(Include = "Price,Description,Type")] House house,string selectedTypeText)
        {
            ModelState["house.Type"].Errors.Clear();
            house.Type = selectedTypeText;
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

        // GET: Houses/Edit/5
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
            NewHouse newHouse = new NewHouse()
            {
                House = house,
                Types = repository.getAvaiableTypes()
            };
            return View(newHouse);
        }

        // POST: Houses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles ="Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Description,Type,Price,SelectedTypeText")] NewHouse house)
        {
            house.House.Type = house.SelectedTypeText;
            if (ModelState.IsValid)
            {
                try
                {
                    repository.UpdateHouse(house.House);
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

        // GET: Houses/Delete/5
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

        //// POST: Houses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            House house = repository.GetHouseById(id);
            repository.setAvailabilityHouse(house);

            if (house.statusHouse.Equals("Zajęty"))
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
