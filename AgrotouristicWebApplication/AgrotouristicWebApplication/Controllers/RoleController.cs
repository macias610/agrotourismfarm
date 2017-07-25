using Microsoft.AspNet.Identity.EntityFramework;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AgrotouristicWebApplication.Controllers
{
    public class RoleController : Controller
    {
        private AgrotourismContext db = new AgrotourismContext();
        
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

            List<IdentityRole> Roles = db.Roles.AsNoTracking().ToList();
            return View(Roles);
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
                if(db.Roles.ToList().Contains(role,new RoleComparer()))
                {
                    ViewBag.error = true;
                    ModelState.Clear();
                    return View();
                }

                try
                {
                    db.Roles.Add(role);
                    db.SaveChanges();
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

        // GET: Role/Edit/5
        [Authorize(Roles ="Admin")]
        public ActionResult Edit(string id)
        {
            if (id.Equals(null))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IdentityRole role = db.Roles.Find(id);
            if (role == null)
            {
                return HttpNotFound();
            }

            return View(role);
        }

        // POST: Role/Edit/5
        [HttpPost]
        [Authorize(Roles ="Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include ="Id,Name")]IdentityRole role)
        {

            if (ModelState.IsValid)
            {

                if (db.Roles.ToList().Contains(role, new RoleComparer()))
                {
                    ViewBag.error = true;
                    ModelState.Clear();
                    return View(role);
                }

                try
                {
                    db.Entry(role).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch 
                {
                    ViewBag.exception = true;
                    return View(role);
                }
            }
            ViewBag.exception = false;
            ViewBag.error = false;
            return View(role);
        }
    
    }

    public class RoleComparer : IEqualityComparer<IdentityRole>
    {
        public bool Equals(IdentityRole x, IdentityRole y)
        {
            return x.Name == y.Name;
        }

        public int GetHashCode(IdentityRole obj)
        {
            return obj.GetHashCode();
        }
    }
}
