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
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Repository.ViewModels;
using Microsoft.Ajax.Utilities;

namespace AgrotouristicWebApplication.Controllers
{
    public class ClientReservationsController : Controller
    {
        private readonly IReservationRepository repository;

        public ClientReservationsController(IReservationRepository repository)
        {
            this.repository = repository;
        }

        [Authorize(Roles ="Klient")]
        public ActionResult Index()
        {
            List<Reservation> reservations = repository.GetClientReservations(User.Identity.GetUserId()).ToList();
            reservations=repository.RemoveOutOfDateReservations(reservations);
            return View(reservations);
        }

        [Authorize(Roles ="Klient")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = repository.GetReservationById((int)id);
            if (reservation == null)
            {
                return HttpNotFound();
            }      
            //List<ReservationAttractionDetails> attractionDetails = new List<ReservationAttractionDetails>();

            //repository.GetAttractionsForReservation(reservation.Id).ForEach(item => attractionDetails.Add(new ReservationAttractionDetails()
            //{
            //    Attraction = item,
            //    Attraction_Reservation = repository.GetDetailsAboutReservedAttraction(item.Id),
            //    Workers = repository.GetWorkersAssignedToAttraction(repository.GetDetailsAboutReservedAttraction(item.Id).Id)
            //}));

            return View(reservation);
        } 

        [Authorize(Roles ="Klient")]
        public ActionResult Create()
        {
            string action = (Request.UrlReferrer.Segments.Skip(2).Take(1).SingleOrDefault() ?? "Index").Trim('/');
            List<string> actions = new List<string>()
            {
                "AddTerm","AddHouses","AddMeals","AddParticipants","AddAttractions"
            };
            if(!actions.Contains(action))
            {
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
            if (ModelState.IsValid)
            {
                reservation = (NewReservation )Session["Reservation"];

                Reservation savedReservation = repository.GetReservationBasedOnData(reservation, User.Identity.GetUserId());
                repository.AddReservation(savedReservation);
                repository.SaveChanges();
                repository.SaveAssignedMealsAndHouses(savedReservation.Id, reservation);
                if(reservation.AssignedAttractions.Any(pair => pair.Value != null && pair.Value.Any()))
                {
                    repository.SaveAssignedAttractions(savedReservation.Id, reservation);
                }

                Session.Remove("Reservation");
                return RedirectToAction("Index");
            }
            return View(reservation);
        }

        [Authorize(Roles ="Klient")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = repository.GetReservationById((int)id);
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
                editedReservation = repository.RetreiveExistingReservation(reservation);
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
            if (ModelState.IsValid)
            {
                reservation = (NewReservation)Session["Reservation"];
                if(reservation.stagesConfirmation[2])
                    repository.ChangeAssignedMeals(Int32.Parse(Request.Form["ReservationId"]), reservation);
                if (reservation.stagesConfirmation[3])
                    repository.ChangeAssignedParticipants(Int32.Parse(Request.Form["ReservationId"]), reservation);
                if (reservation.stagesConfirmation[4])
                    repository.ChangeAssignedAttractions(Int32.Parse(Request.Form["ReservationId"]), reservation);

                Session.Remove("Reservation");
                return RedirectToAction("Index");
            }
            ViewBag.ReservationId = Int32.Parse(Request.Form["ReservationId"]);
            return View(reservation);
        }

        [Authorize(Roles ="Klient")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = repository.GetReservationById((int)id);
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
            Reservation reservation = repository.GetReservationById(id);
            repository.RemoveReservation(reservation);
            repository.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
