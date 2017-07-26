using Microsoft.AspNet.Identity.EntityFramework;
using Repository.IRepo;
using Repository.Repo;
using Repository.Repository;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace AgrotouristicWebApplication.Controllers
{
    public class RoleController : Controller
    {
        private readonly IRoleRepository repository;

        public RoleController(IRoleRepository repository)
        {
            this.repository = repository;
        }

        [Authorize(Roles ="Admin")]
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {


                if (!User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

            IQueryable<IdentityRole> roles = repository.GetRoles();
            return View(roles);
        }

        // GET: Role/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Role/Create
        [HttpPost]
        [Authorize(Roles ="Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include ="Name")] IdentityRole role)
        {
            if (ModelState.IsValid)
            {
                List<IdentityRole> roles = repository.GetRoles().ToList();
                if (roles.Contains(role,new RoleComparer()))
                {
                    ViewBag.error = true;
                    ModelState.Clear();
                    return View();
                }

                try
                {
                    repository.AddRole(role);
                    repository.SaveChanges();
                }
                catch
                {
                    ViewBag.exception = true;
                    return View();
                }
            }
            ViewBag.exception = false;
            ViewBag.error = false;
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            //if (disposing)
            //{
            //    repository.Dispose();
            //}
            base.Dispose(disposing);
        }

    }

    
}
