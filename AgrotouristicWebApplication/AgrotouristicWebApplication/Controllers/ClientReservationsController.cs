using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;

using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PagedList;
using System.Net;
using System.Web;
using Service.IService;
using DomainModel.Models;
using ViewModel;

namespace AgrotouristicWebApplication.Controllers
{
    public class ClientReservationsController : Controller
    {
        private readonly IReservationService reservationService =null;
        private readonly IAttractionReservationService attractionReservationService = null;
        private readonly IParticipantReservationService participantReservationService = null;
        private readonly IMealReservationService mealReservationService = null;

        public ClientReservationsController(IReservationService reservationService, IAttractionReservationService attractionReservationService, IParticipantReservationService participantReservationService, IMealReservationService mealReservationService)
        {
            this.reservationService = reservationService;
            this.attractionReservationService = attractionReservationService;
            this.participantReservationService = participantReservationService;
            this.mealReservationService = mealReservationService;
        }

        [Authorize(Roles ="Klient")]
        public ActionResult Index(int? page)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            reservationService.RemoveOutOfDateReservations(User.Identity.GetUserId());
            IList<Reservation> reservations = reservationService.GetClientReservations(User.Identity.GetUserId());
            int currentPage = page ?? 1;
            int perPage = 4;
            return View(reservations.ToPagedList<Reservation>(currentPage,perPage));
        }

        [Authorize(Roles ="Klient")]
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
            Reservation reservation = reservationService.GetReservationById((int)id);
            if (reservation == null)
            {
                return HttpNotFound();
            }      

            return View(reservation);
        } 

        [Authorize(Roles ="Klient")]
        public ActionResult Create(bool? concurrencyError)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true});
            }

            string action = (Request.UrlReferrer.Segments.Skip(2).Take(1).SingleOrDefault() ?? "Index").Trim('/');
            List<string> actions = new List<string>()
            {
                "AddTerm","AddHouses","AddMeals","AddParticipants","AddAttractions"
            };
            if(!actions.Contains(action))
            {
                Session.Remove("Reservation");
            }
            if(concurrencyError.GetValueOrDefault())
            {
                    ViewBag.ConcurrencyErrorMessage = "Inny użytkownik "
                        + "wcześniej zarezerwował wybrane domki."
                        + "Rezerwację anulowano. "
                        + "Wybierz ponownie";
                Session.Remove("Reservation");
            }
            NewReservation reservation = (NewReservation)Session["Reservation"];
            return View("~/Views/ClientReservations/Create.cshtml",reservation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Klient")]
        public ActionResult Create([Bind(Include = "StartDate,EndDate")] NewReservation reservation)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            if (ModelState.IsValid)
            {
                reservation = (NewReservation )Session["Reservation"];
                if(!reservationService.checkAvaiabilityHousesBeforeConfirmation(reservation))
                {
                    return RedirectToAction("Create", new { concurrencyError = true});
                }

                Reservation savedReservation = reservationService.GetReservationBasedOnData(reservation, User.Identity.GetUserId());
                reservationService.AddReservation(savedReservation);
                reservationService.SaveReservationHouses(savedReservation.Id, reservation);
                if(reservation.AssignedAttractions.Any(pair => pair.Value != null && pair.Value.Any()))
                {
                    attractionReservationService.SaveAssignedAttractions(savedReservation.Id, reservation);
                }

                Session.Remove("Reservation");
                reservationService.SendEmailAwaitingReservation(savedReservation);
                return RedirectToAction("Index");
            }
            return View(reservation);
        }

        [Authorize(Roles ="Klient")]
        public ActionResult Edit(int? id)
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
            Reservation reservation = reservationService.GetReservationById((int)id);
            if (reservation == null)
            {
                return HttpNotFound();
            }
            NewReservation editedReservation = null;
            string action = (Request.UrlReferrer.Segments.Skip(2).Take(1).SingleOrDefault() ?? "Index").Trim('/');

            List<string> actions = new List<string>()
            {
                "EditMeals","EditParticipants","EditAttractions"
            };
            if (!actions.Contains(action))
            {
                Session.Remove("Reservation");
                editedReservation = reservationService.RetreiveExistingReservation(reservation);
                Session["Reservation"] = editedReservation;
            }
            else
            {
                editedReservation = (NewReservation)Session["Reservation"];
            }
            ViewBag.ReservationId = id;
            return View(editedReservation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Klient")]
        public ActionResult Edit([Bind(Include = "StartDate,EndDate")] NewReservation reservation)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            if (ModelState.IsValid)
            {
                reservation = (NewReservation)Session["Reservation"];
                if(reservation.stagesConfirmation[2])
                    this.mealReservationService.ChangeAssignedMeals(Int32.Parse(Request.Form["ReservationId"]), reservation);
                if (reservation.stagesConfirmation[3])
                    this.participantReservationService.ChangeAssignedParticipants(Int32.Parse(Request.Form["ReservationId"]), reservation);
                if (reservation.stagesConfirmation[4])
                    this.attractionReservationService.ChangeAssignedAttractions(Int32.Parse(Request.Form["ReservationId"]), reservation);

                Session.Remove("Reservation");
                return RedirectToAction("Index");
            }
            ViewBag.ReservationId = Int32.Parse(Request.Form["ReservationId"]);
            return View(reservation);
        }

        [Authorize(Roles ="Klient")]
        public ActionResult Delete(int? id)
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
            Reservation reservation = reservationService.GetReservationById((int)id);
            if (reservation == null)
            {
                return HttpNotFound();
            }
            ViewBag.NumberHouses = reservation.Reservation_House.Count;
            return View(reservation);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Klient")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            Reservation reservation = reservationService.GetReservationById(id);
            reservationService.RemoveReservation(reservation);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
