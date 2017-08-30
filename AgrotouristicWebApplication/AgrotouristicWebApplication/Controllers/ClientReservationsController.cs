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
    public class ClientReservationsController : Controller,IController
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
            return View(reservations.ToList());
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

        [HttpPost]
        [Authorize(Roles ="Klient")]
        public ActionResult GetReservationDetails(int id)
        {
            List<House> houses = repository.GetHousesForReservation(id);
            Dictionary<string, List<SelectListItem>> dictionary = new Dictionary<string, List<SelectListItem>>()
            {
                { "Houses",houses.Select(house => new SelectListItem { Value = house.Name , Text = house.Name }).ToList()}
            };
            return PartialView("~/Views/Shared/_ReservationDetailsPartial.cshtml", dictionary);
        }

        [HttpPost]
        [Authorize(Roles ="Klient")]
        public ActionResult GetHouseDetails(int id,string houseName)
        {
            House house = repository.GetHousesForReservation(id).Where(item => item.Name.Equals(houseName)).FirstOrDefault();
            ReservationHouseDetails houseDetails = new ReservationHouseDetails()
            {
                House = house,
                Meal = repository.GetHouseMealForReservation(id,house.Id),
                Participants = repository.GetParticipantsHouseForReservation(id,house.Id)
            };
            return PartialView("~/Views/Shared/_ReservationHouseDetailsPartial.cshtml", houseDetails);
        }

        [HttpPost]
        [Authorize(Roles ="Klient")]
        [ValidateAntiForgeryToken]
        public ActionResult GetAvaiableHouses(DateTime startDate, DateTime endDate)
        {
            Session.Remove("Reservation");
            List<string> houses = new List<string>();
            repository.GetNamesAvaiableHouses(repository.GetAvaiableHousesInTerm(startDate, endDate)).ToList().ForEach(item => houses.Add(item.Value));
            if(houses.Count == 0)
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
        public ActionResult GetHouseParticipants(string houseName, int reservationId)
        {
            NewReservation reservation =(NewReservation)Session["Reservation"];
            List<Participant> participants = reservation.AssignedParticipantsHouses[houseName];
            return PartialView("~/Views/Shared/_HouseParticipantsPartial.cshtml", participants);
        }

        [Authorize(Roles ="Klient")]
        public ActionResult AddTerm()
        {
            return View("~/Views/ClientReservations/AddTerm.cshtml");
        }

        [HttpPost]
        [Authorize(Roles ="Klient")]
        public ActionResult AddTerm(bool isTermConfirmed)
        {
            NewReservation reservation = (NewReservation)Session["Reservation"];
            reservation.stagesConfirmation[0] = isTermConfirmed;
            Session["Reservation"] = reservation;
            return RedirectToAction("Create");
        }
        

        [Authorize(Roles ="Klient")]
        public ActionResult AddHouses()
        {
            NewReservation reservation = (NewReservation)Session["Reservation"];
            Dictionary<string, List<SelectListItem>> dictionary = new Dictionary<string, List<SelectListItem>>()
            {
                { "Avaiable",repository.GetNamesAvaiableHouses(repository.GetAvaiableHousesInTerm(reservation.StartDate, reservation.EndDate))},
                { "Selected",new List<SelectListItem>()}
            };
            ViewBag.OverallCost = reservation.OverallCost;
            ViewBag.StayLength = reservation.EndDate.Subtract(reservation.StartDate).Days + 1;
            return View("~/Views/ClientReservations/AddHouses.cshtml", dictionary);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Klient")]
        public ActionResult AddHouses(bool isHousesConfirmed)
        {
            NewReservation reservation = (NewReservation)Session["Reservation"];
            reservation.OverallCost = Decimal.Parse(Request.Form["housesOverallCost"]);
            repository.SaveSelectedHouses(reservation, Request.Form.GetValues("HousesListBoxSelected").ToList());
            reservation.stagesConfirmation[1] = isHousesConfirmed;
            Session["Reservation"] = reservation;
            return RedirectToAction("Create");
        }

        [Authorize(Roles ="Klient")]
        public ActionResult AddMeals()
        {
            NewReservation reservation = (NewReservation)Session["Reservation"];
            Dictionary<string, List<SelectListItem>> dictionary = new Dictionary<string, List<SelectListItem>>()
            {
                { "AvaiableMeals",repository.GetNamesAvaiableMeals()},
                { "SelectedHouses",reservation.AssignedHousesMeals.Keys.Select(key => new SelectListItem { Value = key, Text = key }).ToList()},
                { "SelectedMeals",new List<SelectListItem>()},
                { "SelectedMealsListBox",new List<SelectListItem>()}
            };
            ViewBag.OverallCost = reservation.OverallCost;
            ViewBag.StayLength = reservation.EndDate.Subtract(reservation.StartDate).Days+1;
            ViewBag.ReservationId = 0;
            return View("~/Views/ClientReservations/AddMeals.cshtml", dictionary);
        }

        [HttpPost]
        [Authorize(Roles ="Klient")]
        [ValidateAntiForgeryToken]
        public ActionResult AddMeals(bool isMealsConfirmed)
        {
            NewReservation reservation = (NewReservation)Session["Reservation"];
            reservation.OverallCost = Decimal.Parse(Request.Form["mealsOverallCost"]);
            repository.SaveAssignedMealsToHouses(reservation, Request.Form.GetValues("ListBoxSelectedHousesMeals").ToList());
            reservation.stagesConfirmation[2] = isMealsConfirmed;
            Session["Reservation"] = reservation;
            return RedirectToAction("Create");
        }

        [Authorize(Roles ="Klient")]
        public ActionResult AddParticipants()
        {
            NewReservation reservation = (NewReservation)Session["Reservation"];
            ViewBag.ReservationId = 0;
            return View("~/Views/ClientReservations/AddParticipants.cshtml", reservation.AssignedParticipantsHouses);
        }

        [Authorize(Roles ="Klient")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddParticipants(bool isParticipantsConfirmed)
        {       
            NewReservation reservation = (NewReservation)Session["Reservation"];
            if(!repository.ValidateFormularParticipants(reservation.AssignedParticipantsHouses))
            {
                ViewBag.error = true;
                return View("~/Views/ClientReservations/AddParticipants.cshtml", reservation.AssignedParticipantsHouses);
            }
            reservation.stagesConfirmation[3] = isParticipantsConfirmed;
            return RedirectToAction("Create");
        }

        [HttpPost]
        [Authorize(Roles ="Klient")]
        [ValidateHeaderAntiForgeryToken]
        public ActionResult ChangeHouseParticipants(string houseName, List<Participant> participants)
        {
            NewReservation reservation = (NewReservation)Session["Reservation"];
            if (reservation.AssignedParticipantsHouses.FirstOrDefault().Value.FirstOrDefault().Id != 0)
                reservation.AssignedParticipantsHouses[houseName] = repository.CopyParticipantsData(reservation.AssignedParticipantsHouses[houseName], participants);
            else
                reservation.AssignedParticipantsHouses[houseName] = participants;
            Session["Reservation"] = reservation;
            return PartialView("~/Views/Shared/_HouseParticipantsPartial.cshtml",participants);
        } 

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
                Session.Remove("Reservation");
                return RedirectToAction("Index");
            }
            return View(reservation);
        }

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
                "EditMeals","EditParticipants"
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
        [Authorize(Roles ="Klient")]
        public ActionResult EditMeals(int id)
        {
            NewReservation reservation = (NewReservation)Session["Reservation"];
            Dictionary<string, List<SelectListItem>> dictionary = new Dictionary<string, List<SelectListItem>>()
            {
                { "AvaiableMeals",repository.GetNamesAvaiableMeals()},
                { "SelectedHouses",new List<SelectListItem>()},
                { "SelectedMeals",repository.GetSelectedHousesMeals(reservation.AssignedHousesMeals,false)},
                { "SelectedMealsListBox",repository.GetSelectedHousesMeals(reservation.AssignedHousesMeals,true)}
            };
            ViewBag.OverallCost = reservation.OverallCost;
            ViewBag.StayLength = reservation.EndDate.Subtract(reservation.StartDate).Days + 1;
            ViewBag.ReservationId = id;
            return View("~/Views/ClientReservations/AddMeals.cshtml", dictionary);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Klient")]
        public ActionResult EditMeals(bool isMealsConfirmed)
        {
            NewReservation reservation = (NewReservation)Session["Reservation"];
            reservation.OverallCost = Decimal.Parse(Request.Form["mealsOverallCost"]);
            repository.SaveAssignedMealsToHouses(reservation, Request.Form.GetValues("ListBoxSelectedHousesMeals").ToList());
            reservation.stagesConfirmation[2] = isMealsConfirmed;
            Session["Reservation"] = reservation;
            return RedirectToAction("Edit", new { id = Int32.Parse(Request.Form["reservationId"]) });
        }

        [Authorize(Roles ="Klient")]
        public ActionResult EditParticipants(int id)
        {
            NewReservation reservation = (NewReservation)Session["Reservation"];
            ViewBag.ReservationId = id;
            return View("~/Views/ClientReservations/AddParticipants.cshtml", reservation.AssignedParticipantsHouses);
        }

        [HttpPost]
        [Authorize(Roles="Klient")]
        [ValidateAntiForgeryToken]
        public ActionResult EditParticipants(bool isParticipantsConfirmed)
        {
            NewReservation reservation = (NewReservation)Session["Reservation"];
            if (!repository.ValidateFormularParticipants(reservation.AssignedParticipantsHouses))
            {
                ViewBag.error = true;
                return View("~/Views/ClientReservations/AddParticipants.cshtml", reservation.AssignedParticipantsHouses);
            }
            reservation.stagesConfirmation[3] = isParticipantsConfirmed;

            return RedirectToAction("Edit", new { id = Int32.Parse(Request.Form["reservationId"]) });
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

                Session.Remove("Reservation");
                return RedirectToAction("Index");
            }
            ViewBag.ReservationId = Int32.Parse(Request.Form["ReservationId"]);
            return View(reservation);
        }

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
