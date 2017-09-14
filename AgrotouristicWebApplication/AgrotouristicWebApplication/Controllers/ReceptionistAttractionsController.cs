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
    public class ReceptionistAttractionsController : Controller
    {
        private readonly IReservedAttractionsRepository repository;

        public ReceptionistAttractionsController(IReservedAttractionsRepository repository)
        {
            this.repository = repository;
        }

        [Authorize(Roles ="Recepcjonista")]
        public ActionResult Index()
        {
            List<SelectListItem> weeksSelectListItem = repository.GetWeeksForAttractions(DateTime.Now);
            return View(weeksSelectListItem);
        }

        [HttpPost]
        [Authorize(Roles ="Recepcjonista")]
        public ActionResult GetAttractionsInWeek(string term)
        {
            AttractionInstructors attractionInstructors = null;
            if (term.Equals("-"))
            {
                return new EmptyResult();
            }
            else
            {
                attractionInstructors = new AttractionInstructors()
                {
                    DaysOfWeek = repository.GetAvaiableDatesInWeek(term),
                    AssignedAttractions = repository.GetAttractionsInstructorsInGivenWeek(term),
                    MaxRows = repository.GetMaxRowsToTableAttractions(repository.GetAttractionsInstructorsInGivenWeek(term))
                };
            }
            return PartialView("~/Views/Shared/_WeeklyTimetableAttractionsInstructorsPartial.cshtml", attractionInstructors);
        }

        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Meal meal = db.Meals.Find(id);
        //    if (meal == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(meal);
        //}

        //public ActionResult Create()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,Type,Price")] Meal meal)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Meals.Add(meal);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(meal);
        //}

        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Meal meal = db.Meals.Find(id);
        //    if (meal == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(meal);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,Type,Price")] Meal meal)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(meal).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(meal);
        //}

        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Meal meal = db.Meals.Find(id);
        //    if (meal == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(meal);
        //}

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Meal meal = db.Meals.Find(id);
        //    db.Meals.Remove(meal);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
