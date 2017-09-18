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
