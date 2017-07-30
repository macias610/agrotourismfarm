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
using Microsoft.AspNet.Identity.EntityFramework;
using Repository.ViewModels;

namespace AgrotouristicWebApplication.Controllers
{
    public class UsersController : Controller
    {
        private readonly IRoleRepository repository;

        public UsersController(IRoleRepository repository)
        {
            this.repository = repository;
        }

        // GET: RoleUsers
        [Authorize(Roles = "Admin")]
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

            List<User> users = repository.GetUsers().ToList();
            Dictionary<string,string> roles = repository.GetRoles().ToDictionary(x=>x.Id,x=>x.Name);
            RolesUsers rolesUsers = new RolesUsers
            {
                Users = users,
                Roles = roles
            };
            return View(rolesUsers);
        }

        // GET: RoleUsers/Create
        public ActionResult Create(string id)
        {
            List<IdentityUserRole> userRoles = repository.GetUserById(id).Roles.ToList();
            Dictionary<string, string> roles = repository.GetRoles().ToDictionary(x => x.Id, x => x.Name);

            RoleUser roleUser = new RoleUser
            {
                userId = id,
                Roles = repository.GetNewRolesForUser(userRoles,roles)
            };
            return View(roleUser);
        }

        // POST: RoleUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Admin")]
        public ActionResult Create([Bind(Include = "userId,selectedRoleText")] RoleUser user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    repository.AssignToRole(user.userId,user.SelectedRoleText);
                    repository.SaveChanges();
                }
                catch
                {
                    ViewBag.exception = true;
                    return View();
                }    
            }
            ViewBag.exception = false;
            return RedirectToAction("Index");
        }


        // GET: RoleUsers/Delete/5
        //public ActionResult Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    User user = db.Users.Find(id);
        //    if (user == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(user);
        //}

        //// POST: RoleUsers/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(string id)
        //{
        //    User user = db.Users.Find(id);
        //    db.Users.Remove(user);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
