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
using Repository.ViewModels;

namespace AgrotouristicWebApplication.Controllers
{
    public class ReceptionistAttractionsController : Controller
    {
        private readonly IReservedAttractionsRepository repository;

        public ReceptionistAttractionsController(IReservedAttractionsRepository repository)
        {
            this.repository = repository;
        }

        [Authorize(Roles ="Recepcjonista")]
        public ActionResult Index()
        {
            List<SelectListItem> weeksSelectListItem = repository.GetWeeksForAttractions(DateTime.Now);
            return View(weeksSelectListItem);
        }

        [HttpPost]
        [Authorize(Roles ="Recepcjonista")]
        public ActionResult GetAttractionsInWeek(string term)
        {
            AttractionInstructors attractionInstructors = null;
            if (term.Equals("-"))
            {
                return new EmptyResult();
            }
            else
            {
                attractionInstructors = new AttractionInstructors()
                {
                    DaysOfWeek = repository.GetAvaiableDatesInWeek(term),
                    AssignedAttractions = repository.GetAttractionsInstructorsInGivenWeek(term),
                    MaxRows = repository.GetMaxRowsToTableAttractions(repository.GetAttractionsInstructorsInGivenWeek(term))
                };
            }
            return PartialView("~/Views/Shared/_WeeklyTimetableAttractionsInstructorsPartial.cshtml", attractionInstructors);
        }

        [Authorize(Roles ="Recepcjonista")]
        public ActionResult RemoveInstructor(int id,string name,int reservationAttractionWorkerId)
        {
            Attraction_Reservation attractionReservation = repository.GetAttractionReservationById(id);
            AttractionRemovedInstructor attractionRemovedInstructor = new AttractionRemovedInstructor()
            {
                AttractionName=attractionReservation.Attraction.Name,
                AttractionReservationID=id,
                QuantityParticipant=attractionReservation.QuantityParticipant,
                TermAffair=attractionReservation.TermAffair,
                Instructor = name,
                AttractionReservationWorkerID=reservationAttractionWorkerId
            };
            return View("~/Views/ReceptionistAttractions/RemoveInstructor.cshtml", attractionRemovedInstructor);
        }

        [HttpPost, ActionName("RemoveInstructor")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Recepcjonista")]
        public ActionResult RemoveInstructorConfirmed(int id)
        {
            Attraction_Reservation_Worker attractionReservationWorker = repository.GetAttractionReservationWorkerById(id);
            try
            {
                repository.RemoveAssignedInstructorAttraction(attractionReservationWorker);
                repository.SaveChanges();
            }
            catch 
            {
                ViewBag.exception = true;
                Attraction_Reservation attractionReservation = repository.GetAttractionReservationById(Int32.Parse(Request.Form["attractionReservationId"].ToString()));
                AttractionRemovedInstructor attractionRemovedInstructor = new AttractionRemovedInstructor()
                {
                    AttractionName = attractionReservation.Attraction.Name,
                    AttractionReservationID = id,
                    QuantityParticipant = attractionReservation.QuantityParticipant,
                    TermAffair = attractionReservation.TermAffair,
                    Instructor = Request.Form["instructorName"].ToString(),
                    AttractionReservationWorkerID = Int32.Parse(Request.Form["attractionReservationWorkerId"].ToString())
                };
                return View("~/Views/ReceptionistAttractions/RemoveInstructor.cshtml", attractionRemovedInstructor);
            }
            ViewBag.exception = false;
            return RedirectToAction("Index");
        }
        
        [Authorize(Roles ="Recepcjonista")]
        public ActionResult AddInstructor(int id,string attractionName)
        {
            Attraction_Reservation attractionReservation = repository.GetAttractionReservationById(id);
            AttractionNewInstructor attractionNewInstructor = new AttractionNewInstructor()
            {
                AttractionName = attractionReservation.Attraction.Name,
                TermAffair = attractionReservation.TermAffair,
                QuantityParticipant = attractionReservation.QuantityParticipant,
                AvaiableInstructors = repository.GetAvaiableInstructors(id, attractionReservation.Attraction.Name),
                AttractionReservationID=attractionReservation.Id
            };
            return View("~/Views/ReceptionistAttractions/AddInstructor.cshtml",attractionNewInstructor);
        }

        [HttpPost]
        [Authorize(Roles ="Recepcjonista")]
        [ValidateAntiForgeryToken]
        public ActionResult AddInstructor(int idAttractionReservation)
        {
            Attraction_Reservation_Worker attractionReservationWorker = new Attraction_Reservation_Worker()
            {
                WorkerId= Request.Form["IdInstructor"].ToString(),
                Attraction_ReservationId=idAttractionReservation
            };
            try
            {
                repository.AssignInstructorToAttraction(attractionReservationWorker);
                repository.SaveChanges();
                
            }
            catch
            {
                ViewBag.exception = true;
                Attraction_Reservation attractionReservation = repository.GetAttractionReservationById(idAttractionReservation);
                AttractionNewInstructor attractionNewInstructor = new AttractionNewInstructor()
                {
                    AttractionName = attractionReservation.Attraction.Name,
                    TermAffair = attractionReservation.TermAffair,
                    QuantityParticipant = attractionReservation.QuantityParticipant,
                    AvaiableInstructors = repository.GetAvaiableInstructors(idAttractionReservation, attractionReservation.Attraction.Name),
                    AttractionReservationID = attractionReservation.Id
                };
                ViewBag.exception = false;
                return View("~/Views/ReceptionistAttractions/AddInstructor.cshtml",attractionNewInstructor);
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
