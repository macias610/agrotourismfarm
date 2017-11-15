using PagedList;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AgrotouristicWebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeService homeService;

        public HomeController(IHomeService repository)
        {
            this.homeService = repository;
        }

        public ActionResult Index(bool? expiredSession)
        {
            if (expiredSession.GetValueOrDefault())
            {
                ViewBag.ExpiredSessionMessage = "Sesja wygasła."
                    + "Zaloguj się powonie.";
            }
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Attractions(int? page)
        {
            IList<string> attractions = homeService.GetAvaiableAttractions();
            int currentPage = page ?? 1;
            int perPage = 4;
            return View(attractions.ToPagedList<string>(currentPage,perPage));
        }

        public ActionResult Houses(int? page)
        {
            IList<string> houses = homeService.GetAvaiableHouses();
            int currentPage = page ?? 1;
            int perPage = 4;
            return View(houses.ToPagedList<string>(currentPage,perPage));
        }

        public ActionResult Meals(int? page)
        {
            IList<string> meals = homeService.GetAvaiableMeals();
            int currentPage = page ?? 1;
            int perPage = 4;
            return View(meals.ToPagedList<string>(currentPage,perPage));
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

    }
}