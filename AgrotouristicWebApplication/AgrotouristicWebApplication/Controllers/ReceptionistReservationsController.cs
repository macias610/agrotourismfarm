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
    public class ReceptionistReservationsController : Controller
    {
        private readonly IReservationRepository repository;
        public ReceptionistReservationsController(IReservationRepository repository)
        {
            this.repository = repository;
        }

        // GET: ReceptionistReservations
        public ActionResult Index()
        {
            List<string> states = Reservation.states.ToList();
            List<SelectListItem> list = states.Select(state => new SelectListItem { Text = state, Value = state, Selected = true }).ToList();
            return View(list);
        }

        [HttpPost]
        [Authorize(Roles = "Recepcjonista")]
        public ActionResult GetReservationsByState(string state)
        {
            List<Reservation> reservations = repository.GetReservationsByState(state).ToList();
            return PartialView("~/Views/Shared/_SelectedStatusReservationsPartial.cshtml", reservations);
        }

        // GET: ReceptionistReservations/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Reservation reservation = db.Reservations.Find(id);
        //    if (reservation == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(reservation);
        //}

        // GET: ReceptionistReservations/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Reservation reservation = db.Reservations.Find(id);
        //    if (reservation == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.ClientId = new SelectList(db.Users, "Id", "Email", reservation.ClientId);
        //    return View(reservation);
        //}

        // POST: ReceptionistReservations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,StartDate,EndDate,DeadlinePayment,Status,OverallCost,ClientId")] Reservation reservation)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(reservation).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.ClientId = new SelectList(db.Users, "Id", "Email", reservation.ClientId);
        //    return View(reservation);
        //}

        // GET: ReceptionistReservations/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Reservation reservation = db.Reservations.Find(id);
        //    if (reservation == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(reservation);
        //}

        // POST: ReceptionistReservations/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Reservation reservation = db.Reservations.Find(id);
        //    db.Reservations.Remove(reservation);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
