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
using Microsoft.AspNet.Identity;

namespace AgrotouristicWebApplication.Controllers
{
    public class ClientArchiveReservationsController : Controller
    {
        private readonly IReservationRepository repository;

        public ClientArchiveReservationsController(IReservationRepository repository)
        {
            this.repository = repository;
        }

        [Authorize(Roles ="Klient")]
        public ActionResult Index()
        {
            List<Reservation_History> reservationsHistory = repository.GetClientArchiveReservations(User.Identity.GetUserId()).ToList();
            return View(reservationsHistory);
        }

        [Authorize(Roles = "Klient")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation_History reservationHistory = repository.GetReservationHistoryById((int)id);
            if (reservationHistory == null)
            {
                return HttpNotFound();
            }
            reservationHistory.ReservedHouses=reservationHistory.ReservedHouses.Replace(";", Environment.NewLine);
            return View(reservationHistory);
        }

            // GET: ClientArchiveReservations/Delete/5
            //public ActionResult Delete(int? id)
            //{
            //    if (id == null)
            //    {
            //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //    }
            //    Reservation_History reservation_History = db.Reservations_History.Find(id);
            //    if (reservation_History == null)
            //    {
            //        return HttpNotFound();
            //    }
            //    return View(reservation_History);
            //}

            // POST: ClientArchiveReservations/Delete/5
            //[HttpPost, ActionName("Delete")]
            //[ValidateAntiForgeryToken]
            //public ActionResult DeleteConfirmed(int id)
            //{
            //    Reservation_History reservation_History = db.Reservations_History.Find(id);
            //    db.Reservations_History.Remove(reservation_History);
            //    db.SaveChanges();
            //    return RedirectToAction("Index");
            //}

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
