using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Service.IService;
using ViewModel;
using DomainModel.Models;

namespace AgrotouristicWebApplication.Controllers
{
    public class ReceptionistAttractionsController : Controller
    {
        private readonly IReservedAttractionsService reservedAttractionsService;

        public ReceptionistAttractionsController(IReservedAttractionsService reservedAttractionsService)
        {
            this.reservedAttractionsService = reservedAttractionsService;
        }

        [Authorize(Roles ="Recepcjonista")]
        public ActionResult Index(bool? concurrencyError)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            IList<SelectListItem> weeksSelectListItem = reservedAttractionsService.GetWeeksForAttractions(DateTime.Now);
            if (concurrencyError.GetValueOrDefault())
            {
                ViewBag.ConcurrencyErrorMessage = "Inny użytkownik "
                    + "przypisał/usunął tego instruktora lub  instruktor został usunięty."
                    + "Operacja anulowana. ";
            }
            return View(weeksSelectListItem);
        }

        [HttpPost]
        [Authorize(Roles ="Recepcjonista")]
        public ActionResult GetAttractionsInWeek(string term)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            AttractionInstructors attractionInstructors = null;
            if (term.Equals("-"))
            {
                return new EmptyResult();
            }
            else
            {
                attractionInstructors = new AttractionInstructors()
                {
                    DaysOfWeek = reservedAttractionsService.GetAvaiableDatesInWeek(term),
                    AssignedAttractions = reservedAttractionsService.GetAttractionsInstructorsInGivenWeek(term),
                    MaxRows = reservedAttractionsService.GetMaxRowsToTableAttractions(reservedAttractionsService.GetAttractionsInstructorsInGivenWeek(term))
                };
            }
            return PartialView("~/Views/Shared/_WeeklyTimetableAttractionsInstructorsPartial.cshtml", attractionInstructors);
        }

        [Authorize(Roles ="Recepcjonista")]
        public ActionResult RemoveInstructor(int id,string name,int reservationAttractionWorkerId)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            AttractionReservation attractionReservation = reservedAttractionsService.GetAttractionReservationById(id);
            AttractionInstructorDetails attractionRemovedInstructor = new AttractionInstructorDetails()
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
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            AttractionReservationWorker attractionReservationWorker = reservedAttractionsService.GetAttractionReservationWorkerById(id);
            try
            {
                if (attractionReservationWorker == null || !reservedAttractionsService.checkStateInstructorToAttraction(attractionReservationWorker))
                {
                    return RedirectToAction("Index", new { concurrencyError = true });
                }
                reservedAttractionsService.RemoveAssignedInstructorAttraction(attractionReservationWorker);
                return RedirectToAction("Index");
            }
            catch 
            {
                ViewBag.exception = true;
                AttractionReservation attractionReservation = reservedAttractionsService.GetAttractionReservationById(Int32.Parse(Request.Form["attractionReservationId"].ToString()));
                AttractionInstructorDetails attractionRemovedInstructor = new AttractionInstructorDetails()
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
        }
        
        [Authorize(Roles ="Recepcjonista")]
        public ActionResult AddInstructor(int id,string attractionName)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            AttractionReservation attractionReservation = reservedAttractionsService.GetAttractionReservationById(id);
            AttractionNewInstructor attractionNewInstructor = new AttractionNewInstructor()
            {
                AttractionName = attractionReservation.Attraction.Name,
                TermAffair = attractionReservation.TermAffair,
                QuantityParticipant = attractionReservation.QuantityParticipant,
                AvaiableInstructors = reservedAttractionsService.GetAvaiableInstructors(id, attractionReservation.Attraction.Name),
                AttractionReservationID=attractionReservation.Id
            };

            return View("~/Views/ReceptionistAttractions/AddInstructor.cshtml",attractionNewInstructor);
        }

        [HttpPost]
        [Authorize(Roles ="Recepcjonista")]
        [ValidateAntiForgeryToken]
        public ActionResult AddInstructor(int idAttractionReservation)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            AttractionReservationWorker attractionReservationWorker = new AttractionReservationWorker()
            {
                WorkerId= Request.Form["IdInstructor"].ToString(),
                Attraction_ReservationId=idAttractionReservation,
            };
            try
            {
                if(reservedAttractionsService.checkStateInstructorToAttraction(attractionReservationWorker) || reservedAttractionsService.GetInstructorAssignedToAttraction(attractionReservationWorker.WorkerId) == null)
                {
                    return RedirectToAction("Index", new { concurrencyError = true });
                }
                reservedAttractionsService.AssignInstructorToAttraction(attractionReservationWorker);
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.exception = true;
                AttractionReservation attractionReservation = reservedAttractionsService.GetAttractionReservationById(idAttractionReservation);
                AttractionNewInstructor attractionNewInstructor = new AttractionNewInstructor()
                {
                    AttractionName = attractionReservation.Attraction.Name,
                    TermAffair = attractionReservation.TermAffair,
                    QuantityParticipant = attractionReservation.QuantityParticipant,
                    AvaiableInstructors = reservedAttractionsService.GetAvaiableInstructors(idAttractionReservation, attractionReservation.Attraction.Name),
                    AttractionReservationID = attractionReservation.Id
                };
                ViewBag.exception = false;
                return View("~/Views/ReceptionistAttractions/AddInstructor.cshtml",attractionNewInstructor);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
