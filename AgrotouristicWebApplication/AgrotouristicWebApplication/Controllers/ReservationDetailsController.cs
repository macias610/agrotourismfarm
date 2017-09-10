using Repository.IRepo;
using Repository.Models;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace AgrotouristicWebApplication.Controllers
{
    public class ReservationDetailsController : Controller
    {
        private readonly IReservationDetailsRepository repository;

        public ReservationDetailsController(IReservationDetailsRepository repository)
        {
            this.repository = repository;
        }

        [HttpPost]
        [Authorize(Roles = "Klient")]
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
        [Authorize(Roles = "Klient")]
        public ActionResult GetHouseDetails(int id, string houseName)
        {
            House house = repository.GetHousesForReservation(id).Where(item => item.Name.Equals(houseName)).FirstOrDefault();
            ReservationHouseDetails houseDetails = new ReservationHouseDetails()
            {
                House = house,
                Meal = repository.GetHouseMealForReservation(id, house.Id),
                Participants = repository.GetParticipantsHouseForReservation(id, house.Id)
            };
            return PartialView("~/Views/Shared/_ReservationHouseDetailsPartial.cshtml", houseDetails);
        }

        [HttpPost]
        [Authorize(Roles = "Klient")]
        [ValidateAntiForgeryToken]
        public ActionResult GetAvaiableHouses(DateTime startDate, DateTime endDate)
        {
            Session.Remove("Reservation");
            List<string> houses = new List<string>();
            repository.GetNamesAvaiableHouses(repository.GetAvaiableHousesInTerm(startDate, endDate)).ToList().ForEach(item => houses.Add(item.Value));
            if (houses.Count == 0)
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

        [Authorize(Roles = "Klient")]
        public ActionResult AddTerm()
        {
            return View("~/Views/ReservationDetails/AddTerm.cshtml");
        }

        [HttpPost]
        [Authorize(Roles = "Klient")]
        public ActionResult AddTerm(bool isTermConfirmed)
        {
            NewReservation reservation = (NewReservation)Session["Reservation"];
            reservation.stagesConfirmation[0] = isTermConfirmed;
            Session["Reservation"] = reservation;
            return RedirectToAction("Create","ClientReservations");
        }

        [Authorize(Roles = "Klient")]
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
            return View("~/Views/ReservationDetails/AddHouses.cshtml", dictionary);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Klient")]
        public ActionResult AddHouses(bool isHousesConfirmed)
        {
            NewReservation reservation = (NewReservation)Session["Reservation"];
            reservation.OverallCost = Decimal.Parse(Request.Form["housesOverallCost"]);
            repository.SaveSelectedHouses(reservation, Request.Form.GetValues("HousesListBoxSelected").ToList());
            reservation.stagesConfirmation[1] = isHousesConfirmed;
            Session["Reservation"] = reservation;
            return RedirectToAction("Create","ClientReservations");
        }
        
        [HttpPost]
        [Authorize(Roles = "Klient")]
        public ActionResult GetHouseDescription(string name)
        {
            House house = repository.GetHouseByName(name);
            return PartialView("~/Views/Shared/_HouseDescriptionPartial.cshtml", house);
        }

        [Authorize(Roles = "Klient")]
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
            ViewBag.StayLength = reservation.EndDate.Subtract(reservation.StartDate).Days + 1;
            ViewBag.ReservationId = 0;
            return View("~/Views/ReservationDetails/AddMeals.cshtml", dictionary);
        }

        [HttpPost]
        [Authorize(Roles = "Klient")]
        [ValidateAntiForgeryToken]
        public ActionResult AddMeals(bool isMealsConfirmed)
        {
            NewReservation reservation = (NewReservation)Session["Reservation"];
            reservation.OverallCost = Decimal.Parse(Request.Form["mealsOverallCost"]);
            repository.SaveAssignedMealsToHouses(reservation, Request.Form.GetValues("ListBoxSelectedHousesMeals").ToList());
            reservation.stagesConfirmation[2] = isMealsConfirmed;
            Session["Reservation"] = reservation;
            return RedirectToAction("Create","ClientReservations");
        }

        [Authorize(Roles = "Klient")]
        public ActionResult AddParticipants()
        {
            NewReservation reservation = (NewReservation)Session["Reservation"];
            ViewBag.ReservationId = 0;
            repository.ClearParticipantsFormular(reservation);
            return View("~/Views/ReservationDetails/AddParticipants.cshtml", reservation.AssignedParticipantsHouses);
        }

        [Authorize(Roles = "Klient")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddParticipants(bool isParticipantsConfirmed)
        {
            NewReservation reservation = (NewReservation)Session["Reservation"];
            if (!repository.ValidateFormularParticipants(reservation.AssignedParticipantsHouses))
            {
                ViewBag.error = true;
                ViewBag.ReservationId = 0;
                return View("~/Views/ReservationDetails/AddParticipants.cshtml", reservation.AssignedParticipantsHouses);
            }
            reservation.stagesConfirmation[3] = isParticipantsConfirmed;
            return RedirectToAction("Create","ClientReservations");
        }

        [Authorize(Roles ="Klient")]
        public ActionResult AddAttractions()
        {
            NewReservation reservation = (NewReservation)Session["Reservation"];
            reservation.AssignedAttractions = repository.InitializeDictionaryForAssignedAttractions(reservation.StartDate, reservation.EndDate);
            List<SelectListItem> weeks = repository.GetWeeksFromSelectedTerm(reservation.StartDate, reservation.EndDate);
            Session["Reservation"] = reservation;
            return View("~/Views/ReservationDetails/AddAttractions.cshtml", weeks);
        }

        [HttpPost]
        [Authorize(Roles ="Klient")]
        [ValidateAntiForgeryToken]
        public ActionResult GetAttractionsForTerm(string term)
        {
            if(term.Equals("-"))
            {
                return new EmptyResult();
            }
            else
            {
                NewReservation reservation = (NewReservation)Session["Reservation"];
                ReservationAttractions reservationAttractions = new ReservationAttractions()
                {
                    DaysOfWeek = repository.GetAvaiableDatesInWeek(term),
                    AvaiableAttractions = repository.GetAvaiableAttractions(),
                    ParticipantsQuantity = repository.GetParticipantsQuantity(reservation.AssignedParticipantsHouses.SelectMany(x=>x.Value).Where(item => !(item.Name.Equals("Brak"))).Count()),
                    AssignedAttractions = repository.GetAttractionsInGivenWeek(term,reservation.AssignedAttractions),
                    OverallCost = reservation.OverallCost,
                    MaxRows = repository.GetMaxRowsToTableAttractions(reservation.AssignedAttractions)
                };
                DateTime dd = DateTime.Parse("2017-09-19");
                DateTime dd2 = DateTime.Parse("2017-09-23");
                reservationAttractions.AssignedAttractions[dd].Add("Garncarstwo,2");
                reservationAttractions.AssignedAttractions[dd2].Add("Jazda konna,2");
                reservationAttractions.MaxRows = repository.GetMaxRowsToTableAttractions(reservation.AssignedAttractions);
                return PartialView("~/Views/Shared/_WeeklyTimetableAttractionsPartial.cshtml", reservationAttractions);
            }
        }

        [HttpPost]
        [Authorize(Roles ="Klient")]
        [ValidateAntiForgeryToken]
        public ActionResult AddSelectedAttraction(string term,DateTime date,string attraction)
        {
            NewReservation reservation = (NewReservation)Session["Reservation"];
            ReservationAttractions reservationAttractions = new ReservationAttractions()
            {
                DaysOfWeek = repository.GetAvaiableDatesInWeek(term),
                AvaiableAttractions = repository.GetAvaiableAttractions(),
                ParticipantsQuantity = repository.GetParticipantsQuantity(reservation.AssignedParticipantsHouses.SelectMany(x => x.Value).Where(item => !(item.Name.Equals("Brak"))).Count()),
                AssignedAttractions = repository.GetAttractionsInGivenWeek(term, reservation.AssignedAttractions),
                OverallCost = reservation.OverallCost,
                MaxRows = repository.GetMaxRowsToTableAttractions(reservation.AssignedAttractions)
            };
            reservation.AssignedAttractions[date].Add(attraction);
            reservationAttractions.AssignedAttractions[date].Add(attraction);
            Session["Reservation"] = reservation;
            return PartialView("~/Views/Shared/_WeeklyTimetableAttractionsPartial.cshtml", reservationAttractions);
        }

        [HttpPost]
        [Authorize(Roles = "Klient")]
        public ActionResult GetHouseParticipants(string houseName)
        {
            NewReservation reservation = (NewReservation)Session["Reservation"];
            List<Participant> participants = reservation.AssignedParticipantsHouses[houseName];
            return PartialView("~/Views/Shared/_HouseParticipantsPartial.cshtml", participants);
        }
        [HttpPost]
        [Authorize(Roles = "Klient")]
        [ValidateHeaderAntiForgeryToken]
        public ActionResult ChangeHouseParticipants(string houseName, List<Participant> participants)
        {
            NewReservation reservation = (NewReservation)Session["Reservation"];
            reservation.AssignedParticipantsHouses[houseName] = repository.CopyParticipantsData(reservation.AssignedParticipantsHouses[houseName], participants);
            Session["Reservation"] = reservation;
            return PartialView("~/Views/Shared/_HouseParticipantsPartial.cshtml", participants);
        }

        [Authorize(Roles = "Klient")]
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
            return View("~/Views/ReservationDetails/AddMeals.cshtml", dictionary);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Klient")]
        public ActionResult EditMeals(bool isMealsConfirmed)
        {
            NewReservation reservation = (NewReservation)Session["Reservation"];
            reservation.OverallCost = Decimal.Parse(Request.Form["mealsOverallCost"]);
            repository.SaveAssignedMealsToHouses(reservation, Request.Form.GetValues("ListBoxSelectedHousesMeals").ToList());
            reservation.stagesConfirmation[2] = isMealsConfirmed;
            Session["Reservation"] = reservation;
            return RedirectToAction("Edit","ClientReservations", new { id = Int32.Parse(Request.Form["reservationId"]) });
        }

        [Authorize(Roles = "Klient")]
        public ActionResult EditParticipants(int id)
        {
            NewReservation reservation = (NewReservation)Session["Reservation"];
            ViewBag.ReservationId = id;
            return View("~/Views/ReservationDetails/AddParticipants.cshtml", reservation.AssignedParticipantsHouses);
        }

        [HttpPost]
        [Authorize(Roles = "Klient")]
        [ValidateAntiForgeryToken]
        public ActionResult EditParticipants(bool isParticipantsConfirmed, int reservationId)
        {
            NewReservation reservation = (NewReservation)Session["Reservation"];
            if (!repository.ValidateFormularParticipants(reservation.AssignedParticipantsHouses))
            {
                ViewBag.error = true;
                ViewBag.ReservationId = reservationId;
                return View("~/Views/ReservationDetails/AddParticipants.cshtml", reservation.AssignedParticipantsHouses);
            }
            reservation.stagesConfirmation[3] = isParticipantsConfirmed;

            return RedirectToAction("Edit","ClientReservations", new { id = Int32.Parse(Request.Form["reservationId"]) });
        }


    }
}