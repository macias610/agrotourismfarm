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
        private readonly IUserRepository repository;

        public RoleController(IUserRepository repository)
        {
            this.repository = repository;
        }

        [Authorize(Roles ="Admin")]
        public ActionResult Index()
        {
            IQueryable<IdentityRole> roles = repository.GetRoles();
            return View(roles);
        }

        public ActionResult Create()
        {
            return View();
        }

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
                    ViewBag.exception = false;
                    ViewBag.error = false;
                    return RedirectToAction("Index");
                }
                catch
                {
                    ViewBag.exception = true;
                    return View();
                }
            }
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }

    
}
