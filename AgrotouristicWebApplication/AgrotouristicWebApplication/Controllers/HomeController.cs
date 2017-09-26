using PagedList;
using Repository.IRepo;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AgrotouristicWebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeRepository repository;
        public HomeController(IHomeRepository repository)
        {
            this.repository = repository;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Attractions(int? page)
        {
            List<string> attractions = repository.GetAvaiableAttractions();
            int currentPage = page ?? 1;
            int perPage = 4;
            return View(attractions.ToPagedList<string>(currentPage,perPage));
        }

        public ActionResult Houses(int? page)
        {
            List<string> houses = repository.GetAvaiableHouses();
            int currentPage = page ?? 1;
            int perPage = 4;
            return View(houses.ToPagedList<string>(currentPage,perPage));
        }

        public ActionResult Meals(int? page)
        {
            List<string> meals = repository.GetAvaiableMeals();
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