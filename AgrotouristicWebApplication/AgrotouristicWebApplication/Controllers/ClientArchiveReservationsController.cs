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
using PagedList;

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
        public ActionResult Index(int? page)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            List<Reservation_History> reservationsHistory = repository.GetClientArchiveReservations(User.Identity.GetUserId()).ToList();
            int currentPage = page ?? 1;
            int perPage = 4;
            return View(reservationsHistory.ToPagedList<Reservation_History>(currentPage,perPage));
        }

        [Authorize(Roles = "Klient")]
        public ActionResult Details(int? id)
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
