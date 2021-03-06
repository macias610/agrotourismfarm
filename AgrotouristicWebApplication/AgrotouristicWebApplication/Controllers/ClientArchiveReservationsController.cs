﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PagedList;
using Service.IService;
using DomainModel.Models;

namespace AgrotouristicWebApplication.Controllers
{
    public class ClientArchiveReservationsController : Controller
    {
        private readonly IReservationHistoryService reservationHistoryService;

        public ClientArchiveReservationsController(IReservationHistoryService reservationHistoryService)
        {
            this.reservationHistoryService = reservationHistoryService;
        }

        [Authorize(Roles ="Klient")]
        public ActionResult Index(int? page)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            IList<ReservationHistory> reservationsHistory = reservationHistoryService.GetClientArchiveReservations(User.Identity.GetUserId()).ToList();
            int currentPage = page ?? 1;
            int perPage = 4;
            return View(reservationsHistory.ToPagedList<ReservationHistory>(currentPage,perPage));
        }

        [Authorize(Roles = "Klient")]
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
            ReservationHistory reservationHistory = reservationHistoryService.GetReservationHistoryById((int)id);
            if (reservationHistory == null)
            {
                return HttpNotFound();
            }
            reservationHistory.ReservedHouses=reservationHistory.ReservedHouses.Replace(";", Environment.NewLine);
            reservationHistory.ReservedAttractions = reservationHistory.ReservedAttractions.Replace(";", Environment.NewLine);
            return View(reservationHistory);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
