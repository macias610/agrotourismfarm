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
using Microsoft.AspNet.Identity;

namespace AgrotouristicWebApplication.Controllers
{
    public class InstructorAttractionsController : Controller
    {
        private readonly IReservedAttractionsRepository repository;

        public InstructorAttractionsController(IReservedAttractionsRepository repository)
        {
            this.repository = repository;
        }

        [Authorize(Roles ="Instruktor")]
        public ActionResult Index()
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            List<SelectListItem> weeksSelectListItem = repository.GetWeeksForAttractions(DateTime.Now);
            return View(weeksSelectListItem);
        }

        [HttpPost]
        [Authorize(Roles = "Instruktor")]
        public ActionResult GetClassesInWeek(string term)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            AttractionInstructors attractionInstructor = null;
            if (term.Equals("-"))
            {
                return new EmptyResult();
            }
            else
            {
                attractionInstructor = new AttractionInstructors()
                {
                    DaysOfWeek = repository.GetAvaiableDatesInWeek(term),
                    AssignedAttractions = repository.GetClassesInstructorInGivenWeek(term,User.Identity.GetUserId()),
                    MaxRows = repository.GetMaxRowsToTableAttractions(repository.GetAttractionsInstructorsInGivenWeek(term))
                };
            }

            return PartialView("~/Views/Shared/_WeeklyTimetableClassesInstructorPartial.cshtml",attractionInstructor);
        }

        [Authorize(Roles ="Instruktor")]
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
            Attraction_Reservation attractionReservation = repository.GetAttractionReservationById((int)id);
            if (attractionReservation == null)
            {
                return HttpNotFound();
            }

            AttractionInstructorDetails attractionInstructorDetails = new AttractionInstructorDetails()
            {
                AttractionName = attractionReservation.Attraction.Name,
                Instructor = repository.RetreiveInstructorsAssignedToAttraction(attractionReservation.Id,attractionReservation.Attraction.Name),
                TermAffair=attractionReservation.TermAffair.Date,
                QuantityParticipant=attractionReservation.QuantityParticipant
            };
            return View("~/Views/InstructorAttractions/Details.cshtml",attractionInstructorDetails);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
