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
    public class InstructorAttractionsController : Controller
    {
        private readonly IReservedAttractionsService reservedAttractionsService;

        public InstructorAttractionsController(IReservedAttractionsService reservedAttractionsService)
        {
            this.reservedAttractionsService = reservedAttractionsService;
        }

        [Authorize(Roles ="Instruktor")]
        public ActionResult Index()
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            IList<SelectListItem> weeksSelectListItem = reservedAttractionsService.GetWeeksForAttractions(DateTime.Now);
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
                    DaysOfWeek = reservedAttractionsService.GetAvaiableDatesInWeek(term),
                    AssignedAttractions = reservedAttractionsService.GetClassesInstructorInGivenWeek(term,User.Identity.GetUserId()),
                    MaxRows = reservedAttractionsService.GetMaxRowsToTableAttractions(reservedAttractionsService.GetAttractionsInstructorsInGivenWeek(term))
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
            Attraction_Reservation attractionReservation = reservedAttractionsService.GetAttractionReservationById((int)id);
            if (attractionReservation == null)
            {
                return HttpNotFound();
            }

            AttractionInstructorDetails attractionInstructorDetails = new AttractionInstructorDetails()
            {
                AttractionName = attractionReservation.Attraction.Name,
                Instructor = reservedAttractionsService.RetreiveInstructorsAssignedToAttraction(attractionReservation.Id,attractionReservation.Attraction.Name),
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
