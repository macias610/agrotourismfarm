using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Infrastructure;
using Microsoft.AspNet.Identity;
using Service.IService;
using DomainModel.Models;
using PagedList;

namespace AgrotouristicWebApplication.Controllers
{
    public class ReceptionistReservationsController : Controller
    {
        private readonly IReservationService reservationService =null;
        private readonly IReservationHistoryService reservationHistoryService = null;

        public ReceptionistReservationsController(IReservationService reservationService, IReservationHistoryService reservationHistoryService)
        {
            this.reservationService = reservationService;
            this.reservationHistoryService = reservationHistoryService;
        }

        [Authorize(Roles ="Recepcjonista")]
        public ActionResult Index()
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            IList<string> optionStates = Reservation.OptionStates;
            IList<SelectListItem> list = optionStates.Select(optionState => new SelectListItem { Text = optionState, Value = optionState, Selected = optionState.Equals("-")?true:false }).ToList();
            return View(list);
        }

        [HttpPost]
        [Authorize(Roles = "Recepcjonista")]
        public ActionResult GetReservationsByState(string state, int? page)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            IList<Reservation> reservations = reservationService.GetReservationsByState(state);
            int currentPage = page ?? 1;
            int perPage = 4;
            return PartialView("~/Views/Shared/_SelectedStatusReservationsPartial.cshtml", reservations.ToPagedList<Reservation>(currentPage, perPage));
        }

        [Authorize(Roles ="Recepcjonista")]
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
            return View("~/Views/ClientReservations/Details.cshtml",reservation);
        }

        [Authorize(Roles ="Recepcjonista")]
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
            Reservation editedReservation = reservationService.GetReservationById((int)id);
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
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            string[] fieldsToBind = new string[] { "Name", "Surname","StartDate","EndDate","Status", "RowVersion" };
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation editedReservation = reservationService.GetReservationById((int)id);
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
                    reservationService.UpdateReservation(editedReservation, rowVersion);
                    if (editedReservation.Status.Equals("zarezerwowano"))
                        reservationService.SendEmailConfirmingReservation(editedReservation);
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
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            try
            {
                reservation.Attraction_Reservation = this.reservationService.GetReservationById(reservation.Id).Attraction_Reservation;
                reservation.Reservation_House = this.reservationService.GetReservationById(reservation.Id).Reservation_House;
                Reservation_History reservationHistory = this.reservationHistoryService.GetReservationHistoryBasedReservation(reservation);
                this.reservationService.RemoveReservation(reservation);
                this.reservationHistoryService.AddReservationHistory(reservationHistory);
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
