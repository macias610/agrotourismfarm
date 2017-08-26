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
        [Authorize(Roles ="Klient")]
        public ActionResult Index()
        {
            List<Reservation> reservations = repository.GetClientReservations(User.Identity.GetUserId()).ToList();
            return View(reservations.ToList());
        }

        // GET: Reservations/Details/5
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
        [Authorize(Roles ="Klient")]
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

        [HttpPost]
        [Authorize(Roles ="Klient")]
        [ValidateAntiForgeryToken]
        public ActionResult GetAvaiableHouses(DateTime startDate, DateTime endDate)
        {
            Session.Remove("Reservation");
            IEnumerable <SelectListItem> avaiableHouses = repository.GetAllNamesAvaiableHouses(repository.GetAvaiableHousesInTerm(startDate, endDate));

            List<string> houses = new List<string>();
            avaiableHouses.ToList().ForEach(item => houses.Add(item.Value));

            if(avaiableHouses.ToList().Count == 0)
            {
                return new EmptyResult();
            }

            Session["Reservation"] = new NewReservation()
            {
                StartDate = startDate,
                EndDate = endDate
            };

            return PartialView("~/Views/Shared/_AvaiableHousesPartial.cshtml", houses);
        }

        [Authorize(Roles ="Klient")]
        public ActionResult GetHouseDescription(string name)
        {
            House house = repository.GetHouseByName(name);
            return PartialView("~/Views/Shared/_HouseDescriptionPartial.cshtml", house);
        }

        [HttpPost]
        [Authorize(Roles = "Klient")]
        public ActionResult GetHouseParticipants(string houseName)
        {
            NewReservation reservation = (NewReservation )Session["Reservation"];
            List<Participant> participants = reservation.AssignedParticipantsHouses[houseName];

            return PartialView("~/Views/Shared/_HouseParticipantsPartial.cshtml", participants);
        }

        [Authorize(Roles ="Klient")]
        public ActionResult AddTerm()
        {
            return View("~/Views/ClientReservations/AddTerm.cshtml");
        }

        [Authorize(Roles ="Klient")]
        public ActionResult AddHouses()
        {
            NewReservation reservation = (NewReservation)Session["Reservation"];

            HousesSelection housesSelection = new HousesSelection()
            {
                AvaiableHouses = repository.GetAllNamesAvaiableHouses(repository.GetAvaiableHousesInTerm(reservation.StartDate, reservation.EndDate)),
                SelectedHouses = new List<SelectListItem>()
            };

            return View("~/Views/ClientReservations/AddHouses.cshtml", housesSelection);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Klient")]
        public ActionResult AddHouses([Bind(Include ="StringSelectedHouses")]HousesSelection housesSelection)
        {
            NewReservation reservation = (NewReservation)Session["Reservation"];
            repository.SaveSelectedHouses(reservation,housesSelection.StringSelectedHouses);
            Session["Reservation"] = reservation;
            return RedirectToAction("Create");
        }

        [Authorize(Roles ="Klient")]
        public ActionResult AddMeals()
        {
            NewReservation reservation = (NewReservation)Session["Reservation"];
            MealsSelection mealsSelection = new MealsSelection()
            {
                AvaiableMeals = repository.GetAllNamesAvaiableMeals(),
                SelectedHouses = reservation.AssignedHousesMeals.Keys.Select(key => new SelectListItem { Value = key, Text = key }).ToList(),
                SelectedMeals = new List<SelectListItem>()
            };

            return View("~/Views/ClientReservations/AddMeals.cshtml", mealsSelection);
        }

        [HttpPost]
        [Authorize(Roles ="Klient")]
        [ValidateAntiForgeryToken]
        public ActionResult AddMeals([Bind(Include = "StringSelectedMeals")] MealsSelection mealsSelection)
        {
            NewReservation reservation = (NewReservation)Session["Reservation"];
            repository.SaveAssignedMealsToHouses(reservation,mealsSelection.StringSelectedMeals);
            return RedirectToAction("Create");
        }

        [Authorize(Roles ="Klient")]
        public ActionResult AddParticipants()
        {
            NewReservation reservation = (NewReservation)Session["Reservation"];
            return View("~/Views/ClientReservations/AddParticipants.cshtml", reservation.AssignedParticipantsHouses);
        }

        [HttpPost]
        [Authorize(Roles ="Klient")]
        [ValidateHeaderAntiForgeryToken]
        public ActionResult ChangeHouseParticipants(string houseName, List<Participant> participants)
        {
            NewReservation reservation = (NewReservation)Session["Reservation"];
            reservation.AssignedParticipantsHouses[houseName] = participants;
            Session["Reservation"] = reservation;
            return PartialView("~/Views/Shared/_HouseParticipantsPartial.cshtml",participants);
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Authorize(Roles ="Klient")]
        public ActionResult AddToSelectedHouses(IEnumerable<SelectListItem> avaiableHouses)
        {
            //SelectListItem item = avaiable.Where(x => x.Selected = true).FirstOrDefault();
            //avaiable.ToList().Remove(item);
            //selected.ToList().Add(item);
            return PartialView("~/Views/Shared/_HousesSelectionPartial.cshtml");
        }

        // GET: Reservations/Create
        [Authorize(Roles ="Klient")]
        public ActionResult Create()
        {
            string action = (Request.UrlReferrer.Segments.Skip(2).Take(1).SingleOrDefault() ?? "Index").Trim('/');
            List<string> actions = new List<string>()
            {
                "AddTerm","AddHouses","AddMeals","AddParticipants"
            };
            if(!actions.Contains(action))
            {
                Session.Remove("Reservation");
            }
            NewReservation reservation = (NewReservation)Session["Reservation"];
            return View();
        }

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

        //POST: Reservations/Delete/5
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
