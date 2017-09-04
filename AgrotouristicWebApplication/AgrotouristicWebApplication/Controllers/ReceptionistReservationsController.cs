﻿using System;
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
            List<string> optionStates = Reservation.OptionStates.ToList();
            List<SelectListItem> list = optionStates.Select(optionState => new SelectListItem { Text = optionState, Value = optionState, Selected = optionState.Equals("-")?true:false }).ToList();
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

        [Authorize(Roles ="Recepcjonista")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation editedReservation = repository.GetReservationById((int)id);
            if (editedReservation == null)
            {
                return HttpNotFound();
            }
            EditionStatusReservation reservation = new EditionStatusReservation()
            {
                Id=editedReservation.Id,
                Name=editedReservation.Client.Name,
                Surname=editedReservation.Client.Surname,
                StartDate=editedReservation.StartDate,
                EndDate=editedReservation.EndDate,
                Status=editedReservation.Status,
                States = Reservation.States.Select(state=>new SelectListItem { Text=state,Value=state,Selected=state.Equals(editedReservation.Status)?true:false}).ToList()
            };
            return View(reservation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Recepcjonista")]
        public ActionResult Edit([Bind(Include = "Id,Name,Surname,StartDate,EndDate,Status")] EditionStatusReservation reservation)
        {
            Reservation editedReservation = repository.GetReservationById(reservation.Id);
            if (ModelState.IsValid)
            {
                try
                {
                    editedReservation.Status = reservation.Status;
                    repository.UpdateReservation(editedReservation);
                    repository.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch 
                {
                    ViewBag.exception = true;
                    reservation.States = Reservation.States.Select(state => new SelectListItem { Text = state, Value = state, Selected = state.Equals(editedReservation.Status) ? true : false }).ToList();
                    return View(reservation);
                }
                
            }
            reservation.States = Reservation.States.Select(state => new SelectListItem { Text = state, Value = state, Selected = state.Equals(editedReservation.Status) ? true : false }).ToList();
            return View(reservation);
        }

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
