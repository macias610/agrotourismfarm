using DomainModel.Models;
using HtmlAgilityPack;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.AspNet.Identity;
using NReco.PdfGenerator;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ViewModel;

namespace AgrotouristicWebApplication.Controllers
{
    public class ReservationDetailsController : Controller
    {
        private readonly IReservationDetailsService reservationDetailsService = null;

        public ReservationDetailsController(IReservationDetailsService reservationDetailsService)
        {
            this.reservationDetailsService = reservationDetailsService;
        }

        [HttpPost]
        [Authorize(Roles = "Klient,Recepcjonista")]
        public ActionResult GetReservationDetails(int id)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            IList<House> houses = reservationDetailsService.GetHousesForReservation(id);
            Reservation reservation = reservationDetailsService.GetReservationById(id);
            Dictionary<string, List<SelectListItem>> dictionary = new Dictionary<string, List<SelectListItem>>()
            {
                { "Houses",houses.Select(house => new SelectListItem { Value = house.Name , Text = house.Name }).ToList()},
                { "Weeks",reservationDetailsService.GetWeeksFromSelectedTerm(reservation.StartDate, reservation.EndDate).ToList()}
            };
            return PartialView("~/Views/Shared/_ReservationDetailsPartial.cshtml", dictionary);
        }


        [HttpPost]
        [Authorize(Roles = "Klient,Recepcjonista")]
        public FileResult ExportPDFReservation(int id)
        {
            Reservation reservation = reservationDetailsService.GetReservationById(id);
            string htmlContent = this.RenderView("~/Views/Shared/_PDFReservationPartial.cshtml", reservation);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);
            HtmlNode node = doc.GetElementbyId("DetailsToPDF");
            HtmlToPdfConverter htmlToPdf = new HtmlToPdfConverter();

            node.InnerHtml= "<html><head><meta charset='UTF-8'/></head><body>" + node.InnerHtml + "</body></html>";
            var pdfBytes = htmlToPdf.GeneratePdf("<html><body>" + node.InnerHtml + "</body></html>");
            var contentDisposition = new System.Net.Mime.ContentDisposition
            {
                FileName = "ReservationPDF.pdf",
                Inline = false,
                CreationDate = DateTime.Now,
            };
            Response.ContentEncoding = Encoding.UTF8;
            Response.AppendHeader("Content-Disposition", contentDisposition.ToString());
            return File(pdfBytes, "application/pdf");
        }

        [HttpPost]
        [Authorize(Roles = "Klient,Recepcjonista")]
        public ActionResult GetHouseDetails(int id, string houseName)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            House house = reservationDetailsService.GetHousesForReservation(id).Where(item => item.Name.Equals(houseName)).FirstOrDefault();
            ReservationHouseDetails houseDetails = new ReservationHouseDetails()
            {
                House = house,
                Meal = reservationDetailsService.GetHouseMealForReservation(id, house.Id),
                Participants = reservationDetailsService.GetParticipantsHouseForReservation(id, house.Id)
            };
            return PartialView("~/Views/Shared/_ReservationHouseDetailsPartial.cshtml", houseDetails);
        }

        [HttpPost]
        [Authorize(Roles = "Klient")]
        [ValidateAntiForgeryToken]
        public ActionResult GetAvaiableHouses(DateTime startDate, DateTime endDate)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            Session.Remove("Reservation");
            List<string> houses = new List<string>();
            reservationDetailsService.GetNamesAvaiableHouses(reservationDetailsService.GetAvaiableHousesInTerm(startDate, endDate)).ToList().ForEach(item => houses.Add(item.Value));
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
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            return View("~/Views/ReservationDetails/AddTerm.cshtml");
        }

        [HttpPost]
        [Authorize(Roles = "Klient")]
        public ActionResult AddTerm(bool isTermConfirmed)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            NewReservation reservation = (NewReservation)Session["Reservation"];
            reservation.stagesConfirmation[0] = isTermConfirmed;
            Session["Reservation"] = reservation;
            return RedirectToAction("Create","ClientReservations");
        }

        [Authorize(Roles = "Klient")]
        public ActionResult AddHouses()
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            NewReservation reservation = (NewReservation)Session["Reservation"];
            Dictionary<string, List<SelectListItem>> dictionary = new Dictionary<string, List<SelectListItem>>()
            {
                { "Avaiable",reservationDetailsService.GetNamesAvaiableHouses(reservationDetailsService.GetAvaiableHousesInTerm(reservation.StartDate, reservation.EndDate)).ToList()},
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
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            NewReservation reservation = (NewReservation)Session["Reservation"];
            reservation.OverallCost = Decimal.Parse(Request.Form["housesOverallCost"]);
            reservationDetailsService.SaveSelectedHouses(reservation, Request.Form.GetValues("HousesListBoxSelected").ToList());
            reservation.stagesConfirmation[1] = isHousesConfirmed;
            Session["Reservation"] = reservation;
            return RedirectToAction("Create","ClientReservations");
        }
        
        [HttpPost]
        [Authorize(Roles = "Klient")]
        public ActionResult GetHouseDescription(string name)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            House house = reservationDetailsService.GetHouseByName(name);
            return PartialView("~/Views/Shared/_HouseDescriptionPartial.cshtml", house);
        }

        [Authorize(Roles = "Klient")]
        public ActionResult AddMeals()
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            NewReservation reservation = (NewReservation)Session["Reservation"];
            Dictionary<string, List<SelectListItem>> dictionary = new Dictionary<string, List<SelectListItem>>()
            {
                { "AvaiableMeals",reservationDetailsService.GetNamesAvaiableMeals().ToList()},
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
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            NewReservation reservation = (NewReservation)Session["Reservation"];
            reservation.OverallCost = Decimal.Parse(Request.Form["mealsOverallCost"]);
            reservationDetailsService.SaveAssignedMealsToHouses(reservation, Request.Form.GetValues("ListBoxSelectedHousesMeals").ToList());
            reservation.stagesConfirmation[2] = isMealsConfirmed;
            Session["Reservation"] = reservation;
            return RedirectToAction("Create","ClientReservations");
        }

        [Authorize(Roles = "Klient")]
        public ActionResult AddParticipants()
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            NewReservation reservation = (NewReservation)Session["Reservation"];
            ViewBag.ReservationId = 0;
            reservationDetailsService.ClearParticipantsFormular(reservation);
            return View("~/Views/ReservationDetails/AddParticipants.cshtml", reservation.AssignedParticipantsHouses);
        }

        [Authorize(Roles = "Klient")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddParticipants(bool isParticipantsConfirmed)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            NewReservation reservation = (NewReservation)Session["Reservation"];
            if (!reservationDetailsService.ValidateFormularParticipants(reservation.AssignedParticipantsHouses))
            {
                ViewBag.error = true;
                ViewBag.ReservationId = 0;
                return View("~/Views/ReservationDetails/AddParticipants.cshtml", reservation.AssignedParticipantsHouses);
            }
            reservation.stagesConfirmation[3] = isParticipantsConfirmed;
            Session["Reservation"] = reservation;
            return RedirectToAction("Create","ClientReservations");
        }

        [Authorize(Roles ="Klient")]
        public ActionResult AddAttractions()
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            NewReservation reservation = (NewReservation)Session["Reservation"];
            reservation.AssignedAttractions = reservationDetailsService.InitializeDictionaryForAssignedAttractions(reservation.StartDate, reservation.EndDate);
            IList<SelectListItem> weeks = reservationDetailsService.GetWeeksFromSelectedTerm(reservation.StartDate, reservation.EndDate);
            Session["Reservation"] = reservation;
            ViewBag.OverallCost = reservation.OverallCost;
            ViewBag.ReservationId = 0;
            return View("~/Views/ReservationDetails/AddAttractions.cshtml", weeks);
        }

        [HttpPost]
        [Authorize(Roles ="Klient")]
        [ValidateAntiForgeryToken]
        public ActionResult AddAttractions(bool isAttractionsConfirmed)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            NewReservation reservation = (NewReservation)Session["Reservation"];
            reservation.stagesConfirmation[4] = isAttractionsConfirmed;
            Session["Reservation"] = reservation;
            return RedirectToAction("Create", "ClientReservations");
        }

        [HttpPost]
        [Authorize(Roles ="Klient")]
        [ValidateAntiForgeryToken]
        public ActionResult GetAttractionsForTerm(string term)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            if (term.Equals("-"))
            {
                return new EmptyResult();
            }
            else
            {
                NewReservation reservation = (NewReservation)Session["Reservation"];
                ReservationAttractions reservationAttractions = new ReservationAttractions()
                {
                    DaysOfWeek = reservationDetailsService.GetAvaiableDatesInWeek(term),
                    AvaiableAttractions = reservationDetailsService.GetAvaiableAttractions(),
                    ParticipantsQuantity = reservationDetailsService.GetParticipantsQuantity(reservation.AssignedParticipantsHouses.SelectMany(x=>x.Value).Where(item => !(item.Name.Equals("Brak"))).Count()),
                    AssignedAttractions = reservationDetailsService.GetAttractionsInGivenWeek(term,reservation.AssignedAttractions),
                    OverallCost = reservation.OverallCost,
                    MaxRows = reservationDetailsService.GetMaxRowsToTableAttractions(reservationDetailsService.GetAttractionsInGivenWeek(term, reservation.AssignedAttractions))
                };
                ViewBag.IsOnlyToRead = false;
                return PartialView("~/Views/Shared/_WeeklyTimetableAttractionsPartial.cshtml", reservationAttractions);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Klient,Recepcjonista")]
        [ValidateAntiForgeryToken]
        public ActionResult RetreiveAttractionsForTerm(string term,int id)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            ReservationAttractions reservationAttractions = null;
            if (term.Equals("-"))
            {
                return new EmptyResult();
            }
            else
            {
                reservationAttractions = new ReservationAttractions()
                {
                    DaysOfWeek = reservationDetailsService.GetAvaiableDatesInWeek(term),
                    AvaiableAttractions = reservationDetailsService.GetAvaiableAttractions(),
                    ParticipantsQuantity = reservationDetailsService.GetParticipantsQuantity(reservationDetailsService.RetreiveHouseParticipants(id).SelectMany(x => x.Value).Where(item => !(item.Name.Equals("Brak"))).Count()),
                    AssignedAttractions = reservationDetailsService.RetreiveAttractionsInGivenWeek(term,id),
                    OverallCost = 0,
                    MaxRows = reservationDetailsService.GetMaxRowsToTableAttractions(reservationDetailsService.GetAttractionsInGivenWeek(term, reservationDetailsService.RetreiveAttractionsInGivenWeek(term, id)))
                };
            }
            ViewBag.IsOnlyToRead = true;
            return PartialView("~/Views/Shared/_WeeklyTimetableAttractionsPartial.cshtml", reservationAttractions);
        }

        [HttpPost]
        [Authorize(Roles ="Klient")]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeSelectedAttraction(string term,DateTime date,string attraction)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            NewReservation reservation = (NewReservation)Session["Reservation"];
            if(attraction.Split(';')[1].Equals("Add"))
            {
                reservation.AssignedAttractions[date].Add(attraction.Split(';')[0]);
                decimal price = reservationDetailsService.GetAttractionByName(attraction.Split(',')[0]).Price * Int32.Parse(attraction.Split(';')[0].Split(',')[1]);
                reservation.OverallCost += price;
            }
            else if(attraction.Split(';')[1].Equals("Remove"))
            {
                reservation.AssignedAttractions[date].Remove(attraction.Split(';')[0]);
                decimal price = reservationDetailsService.GetAttractionByName(attraction.Split(',')[0]).Price * Int32.Parse(attraction.Split(';')[0].Split(',')[1]);
                reservation.OverallCost -= price;
                
            }
            Session["Reservation"] = reservation;
            ReservationAttractions reservationAttractions = new ReservationAttractions()
            {
                DaysOfWeek = reservationDetailsService.GetAvaiableDatesInWeek(term),
                AvaiableAttractions = reservationDetailsService.GetAvaiableAttractions(),
                ParticipantsQuantity = reservationDetailsService.GetParticipantsQuantity(reservation.AssignedParticipantsHouses.SelectMany(x => x.Value).Where(item => !(item.Name.Equals("Brak"))).Count()),
                AssignedAttractions = reservationDetailsService.GetAttractionsInGivenWeek(term, reservation.AssignedAttractions),
                OverallCost = reservation.OverallCost,
                MaxRows = reservationDetailsService.GetMaxRowsToTableAttractions(reservation.AssignedAttractions)
            };
            ViewBag.IsOnlyToRead = false;
            ViewBag.OverallCost = reservation.OverallCost;
            return PartialView("~/Views/Shared/_WeeklyTimetableAttractionsPartial.cshtml", reservationAttractions);
        }

        [HttpPost]
        [Authorize(Roles = "Klient")]
        public ActionResult GetHouseParticipants(string houseName)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            NewReservation reservation = (NewReservation)Session["Reservation"];
            IList<Participant> participants = reservation.AssignedParticipantsHouses[houseName];
            return PartialView("~/Views/Shared/_HouseParticipantsPartial.cshtml", participants);
        }
        [HttpPost]
        [Authorize(Roles = "Klient")]
        [ValidateHeaderAntiForgeryToken]
        public ActionResult ChangeHouseParticipants(string houseName, List<Participant> participants)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            NewReservation reservation = (NewReservation)Session["Reservation"];
            reservation.AssignedParticipantsHouses[houseName] = reservationDetailsService.CopyParticipantsData(reservation.AssignedParticipantsHouses[houseName], participants).ToList();
            Session["Reservation"] = reservation;
            return PartialView("~/Views/Shared/_HouseParticipantsPartial.cshtml", participants);
        }

        [Authorize(Roles = "Klient")]
        public ActionResult EditMeals(int id)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            NewReservation reservation = (NewReservation)Session["Reservation"];
            Dictionary<string, List<SelectListItem>> dictionary = new Dictionary<string, List<SelectListItem>>()
            {
                { "AvaiableMeals",reservationDetailsService.GetNamesAvaiableMeals().ToList()},
                { "SelectedHouses",new List<SelectListItem>()},
                { "SelectedMeals",reservationDetailsService.GetSelectedHousesMeals(reservation.AssignedHousesMeals,false).ToList()},
                { "SelectedMealsListBox",reservationDetailsService.GetSelectedHousesMeals(reservation.AssignedHousesMeals,true).ToList()}
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
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            NewReservation reservation = (NewReservation)Session["Reservation"];
            reservation.OverallCost = Decimal.Parse(Request.Form["mealsOverallCost"]);
            reservationDetailsService.SaveAssignedMealsToHouses(reservation, Request.Form.GetValues("ListBoxSelectedHousesMeals").ToList());
            reservation.stagesConfirmation[2] = isMealsConfirmed;
            Session["Reservation"] = reservation;
            return RedirectToAction("Edit","ClientReservations", new { id = Int32.Parse(Request.Form["reservationId"]) });
        }

        [Authorize(Roles = "Klient")]
        public ActionResult EditParticipants(int id)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            NewReservation reservation = (NewReservation)Session["Reservation"];
            ViewBag.ReservationId = id;
            return View("~/Views/ReservationDetails/AddParticipants.cshtml", reservation.AssignedParticipantsHouses);
        }

        [HttpPost]
        [Authorize(Roles = "Klient")]
        [ValidateAntiForgeryToken]
        public ActionResult EditParticipants(bool isParticipantsConfirmed, int reservationId)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            NewReservation reservation = (NewReservation)Session["Reservation"];
            if (!reservationDetailsService.ValidateFormularParticipants(reservation.AssignedParticipantsHouses))
            {
                ViewBag.error = true;
                ViewBag.ReservationId = reservationId;
                return View("~/Views/ReservationDetails/AddParticipants.cshtml", reservation.AssignedParticipantsHouses);
            }
            reservation.stagesConfirmation[3] = isParticipantsConfirmed;

            return RedirectToAction("Edit","ClientReservations", new { id = Int32.Parse(Request.Form["reservationId"]) });
        }

        [Authorize(Roles ="Klient")]
        public ActionResult EditAttractions(int id)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            NewReservation reservation = (NewReservation)Session["Reservation"];
            IList<SelectListItem> weeks = reservationDetailsService.GetWeeksFromSelectedTerm(reservation.StartDate, reservation.EndDate);
            ViewBag.OverallCost = reservation.OverallCost;
            ViewBag.ReservationId = id;
            return View("~/Views/ReservationDetails/AddAttractions.cshtml", weeks);
        }

        [HttpPost]
        [Authorize(Roles = "Klient")]
        [ValidateAntiForgeryToken]
        public ActionResult EditAttractions(bool isAttractionsConfirmed)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            NewReservation reservation = (NewReservation)Session["Reservation"];
            reservation.stagesConfirmation[4] = isAttractionsConfirmed;
            Session["Reservation"] = reservation;
            return RedirectToAction("Edit", "ClientReservations", new { id = Int32.Parse(Request.Form["reservationId"]) });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}