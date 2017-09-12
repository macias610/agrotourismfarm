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
            reservationHistory.ReservedAttractions = reservationHistory.ReservedAttractions.Replace(";", Environment.NewLine);
            return View(reservationHistory);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
