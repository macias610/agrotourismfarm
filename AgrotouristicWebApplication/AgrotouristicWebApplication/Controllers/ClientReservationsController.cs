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
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Microsoft.Ajax.Utilities;
using Repository.Repo;

namespace AgrotouristicWebApplication.Controllers
{
    public class ClientReservationsController : Controller,IController
    {
        private readonly IReservationRepository repository;

        public ClientReservationsController(IReservationRepository repository)
        {
            this.repository = repository;
        }

        // GET: Reservations
        public ActionResult Index()
        {
            List<Reservation> reservations = repository.GetClientReservations(User.Identity.GetUserId()).ToList();
            return View(reservations.ToList());
        }

        // GET: Reservations/Details/5
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
        
            List<ReservationAttractionDetails> attractionDetails = new List<ReservationAttractionDetails>();

            repository.GetAttractionsForReservation(reservation.Id).ForEach(item => attractionDetails.Add(new ReservationAttractionDetails()
            {
                Attraction = item,
                Attraction_Reservation = repository.GetDetailsAboutReservedAttraction(item.Id),
                Workers = repository.GetWorkersAssignedToAttraction(repository.GetDetailsAboutReservedAttraction(item.Id).Id)
            }));

            ReservationDetails reservationDetails = new ReservationDetails()
            {
                Reservation = reservation,
                Houses = repository.GetAllNamesReservedHouses(new List<string>(repository.ConvertToDictionaryHouseDetails(repository.GetHousesForReservation(reservation.Id)).Keys)),
                ReservationAttractionDetails = attractionDetails            
            };
            reservationDetails.SelectedHouseDetailsText = reservationDetails.Houses.First().Value;
            return View(reservationDetails);
        }


        [HttpPost]
        public ActionResult AjaxMethod(int id,string name)
        {
            ReservationHouseDetails houseDetails=null;
            repository.GetHousesForReservation(id).Where(item => item.Name.Equals(name)).ForEach(item => houseDetails = new ReservationHouseDetails()
            {
                House=item,
                Meal = repository.GetHouseMealForReservation(item.Id),
                Participants = repository.GetParticipantsHouseForReservation(item.Id)
            });

            if (name.Equals("-"))
            {
                return new EmptyResult();
            }

            return PartialView("~/Views/Shared/_ReservationHouseDetailsPartial.cshtml", houseDetails);
        }



        // GET: Reservations/Create
        //public ActionResult Create()
        //{
        //    ViewBag.ClientId = new SelectList(db.Users, "Id", "Email");
        //    return View();
        //}

        // POST: Reservations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,StartDate,EndDate,DeadlinePayment,Status,OverallCost,ClientId")] Reservation reservation)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Reservations.Add(reservation);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.ClientId = new SelectList(db.Users, "Id", "Email", reservation.ClientId);
        //    return View(reservation);
        //}

        // GET: Reservations/Edit/5
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

        // POST: Reservations/Edit/5
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

        // GET: Reservations/Delete/5
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

        // POST: Reservations/Delete/5
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
