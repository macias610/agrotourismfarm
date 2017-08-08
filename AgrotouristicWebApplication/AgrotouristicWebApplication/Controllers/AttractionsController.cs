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
    public class AttractionsController : Controller
    {
        private readonly IAttractionRepository repository;

        public AttractionsController(IAttractionRepository repository)
        {
            this.repository = repository;
        }

        // GET: Attractions
        [Authorize(Roles ="Admin")]
        public ActionResult Index()
        {
            List<Attraction> attractions = repository.GetAttractions().ToList();
            return View(attractions);
        }

        // GET: Attractions/Create
        [Authorize(Roles ="Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Attractions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Attractions/Edit/5
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

        // POST: Attractions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles ="Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,Price,Discount")] Attraction attraction)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    repository.UpdateAttraction(attraction);
                    repository.SaveChanges();
                    ViewBag.exception = false;
                    return RedirectToAction("Index");
                }
                catch
                {
                    ViewBag.exception = true;
                    return View(attraction);
                }
                
            }
            return View(attraction);
        }

        // GET: Attractions/Delete/5
        [Authorize(Roles ="Admin")]
        public ActionResult Delete(int? id)
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

        // POST: Attractions/Delete/5
        [Authorize(Roles ="Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Attraction attraction = repository.GetAttractionById(id);

            if (repository.countReservationsWithGivenAttraction(id) >= 1)
            {
                ViewBag.error = true;
                return View(attraction);
            }

            try
            {
                repository.RemoveAttraction(attraction);
                repository.SaveChanges();
            }
            catch
            {
                ViewBag.exception = true;
                return View(attraction);
            }
            ViewBag.exception = false;
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
