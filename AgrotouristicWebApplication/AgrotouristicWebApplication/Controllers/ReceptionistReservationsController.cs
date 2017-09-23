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
using System.Data.Entity.Infrastructure;

namespace AgrotouristicWebApplication.Controllers
{
    public class ReceptionistReservationsController : Controller
    {
        private readonly IReservationRepository repository;
        public ReceptionistReservationsController(IReservationRepository repository)
        {
            this.repository = repository;
        }

        [Authorize(Roles ="Recepcjonista")]
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

        [Authorize(Roles ="Recepcjonista")]
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
            return View("~/Views/ClientReservations/Details.cshtml",reservation);
        }

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
            IList<string> states = Reservation.States;
            ViewData["States"] = states;
            return View(editedReservation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Recepcjonista")]
        public ActionResult Edit(int? id, byte[] rowVersion) 
        {
            string[] fieldsToBind = new string[] { "Name", "Surname","StartDate","EndDate","Status", "RowVersion" };
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation editedReservation = repository.GetReservationById((int)id);
            if (editedReservation == null)
            {
                Reservation deletedReservation = new Reservation();
                TryUpdateModel(deletedReservation, fieldsToBind);
                deletedReservation.Client = new User();
                deletedReservation.Client.Name = Request.Form["Client.Name"];
                deletedReservation.Client.Surname = Request.Form["Client.Surname"];
                ModelState.AddModelError(string.Empty,
                    "Nie można zapisać. Rezerwacja została usunięta przez innego użytkownika.");
                IList<string> ss = Reservation.States;
                ViewData["States"] = ss;
                return View(deletedReservation);
            }
            if (TryUpdateModel(editedReservation, fieldsToBind))
            {
                try
                {
                    repository.UpdateReservation(editedReservation, rowVersion);
                    repository.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    DbEntityEntry entry = ex.Entries.Single();
                    Reservation clientValues = (Reservation)entry.Entity;
                    DbPropertyValues databaseEntry = entry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty,
                            "Nie można zapisać. Rezerwacja została usunięta przez innego użytkownika.");
                    }
                    else
                    {
                        Reservation databaseValues = (Reservation)databaseEntry.ToObject();

                        if (databaseValues.Status != clientValues.Status)
                            ModelState.AddModelError("Status", "Aktulalna wartość: "
                                + databaseValues.Status);
                        if (databaseValues.StartDate != clientValues.StartDate)
                            ModelState.AddModelError("Początek", "Aktulalna wartość: "
                                + databaseValues.StartDate);
                        if (databaseValues.EndDate != clientValues.EndDate)
                            ModelState.AddModelError("Koniec", "Aktulalna wartość: "
                                + databaseValues.EndDate);
                        if (databaseValues.DeadlinePayment != clientValues.DeadlinePayment)
                            ModelState.AddModelError("Termin płatności", "Aktulalna wartość: "
                                + String.Format("{0:c}", databaseValues.DeadlinePayment));
                        if (databaseValues.OverallCost != clientValues.OverallCost)
                            ModelState.AddModelError("Całkowity koszt", "Aktulalna wartość: "
                                + String.Format("{0:c}", databaseValues.OverallCost));
                        ModelState.AddModelError(string.Empty, "Edytowany rekord  "
                            + "został wcześniej zmodyfikowany przez innego użytkownika,operacja anulowana. Pobrano wpisane wartości. Kliknij zapisz, żeby napisać.");
                        editedReservation.RowVersion = databaseValues.RowVersion;
                    }
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Nie można zapisać. Spróbuj ponownie");
                }
            }
            IList<string> states = Reservation.States;
            ViewData["States"] = states;
            return View(editedReservation);
        }

        [Authorize(Roles ="Recepcjonista")]
        public ActionResult Delete(int? id, bool? concurrencyError)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = repository.GetReservationById((int)id);
            if (reservation == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("Index");
                }
                return HttpNotFound();
            }
            if (concurrencyError.GetValueOrDefault())
            {
                ViewBag.ConcurrencyErrorMessage = "Usuwany rekord "
                    + "został zmodyfikowany po pobraniu oryginalnych wartości."
                    + "Usuwanie anulowano i wyświetlono aktualne dane. "
                    + "W celu usunięcia kliknij usuń";
            }
            ViewBag.NumberHouses = reservation.Reservation_House.Count;
            return View(reservation);
        }

        [Authorize(Roles ="Recepcjonista")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed([Bind(Include = "Id,Name,Surname,StartDate,EndDate,OverallCost,ClientId,RowVersion")]Reservation reservation)
        {
            try
            {
                reservation.Attraction_Reservation = repository.GetReservationById(reservation.Id).Attraction_Reservation;
                reservation.Reservation_House = repository.GetReservationById(reservation.Id).Reservation_House;
                Reservation_History reservationHistory = repository.GetReservationHistoryBasedReservation(reservation);
                repository.RemoveReservation(reservation);
                repository.AddReservationHistory(reservationHistory);
                repository.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction("Delete", new { concurrencyError = true, id = reservation.Id });
            }
            catch (DataException)
            {
                ModelState.AddModelError(string.Empty, "Nie można usunąć.Spróbuj ponownie.");
                return View(reservation);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
