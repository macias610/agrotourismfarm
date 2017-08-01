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
    public class RoleUsersController : Controller
    {
        private readonly IUserRepository repository;

        public RoleUsersController(IUserRepository repository)
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

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

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
        [Authorize(Roles ="Admin")]
        public ActionResult Delete(string userId,string SelectedRoleText)
        {
            if (SelectedRoleText == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (userId == null)
            {
                return HttpNotFound();
            }

            RoleUser roleUser = new RoleUser
            {
                userId = userId,
                SelectedRoleText = SelectedRoleText

            };
            return View(roleUser);
        }

        // POST: RoleUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Admin")]
        public ActionResult DeleteConfirmed([Bind(Include = "userId,SelectedRoleText")] RoleUser roleUser)
        {

            Dictionary<string, string> roles = repository.GetRoles().ToDictionary(x => x.Name, x => x.Id);

            if (roleUser.SelectedRoleText.Equals("Admin") && repository.GetNumberOfUsersForGivenRole(roles, "Admin") <= 1)
            {
                ViewBag.error = true;
                return View(roleUser);
            }


            try
            {
                repository.RemoveFromRole(roleUser.userId, roleUser.SelectedRoleText);
                repository.SaveChanges();
            }
            catch
            {
                ViewBag.exception = true;
                return View();
            }
            ViewBag.error = false;
            ViewBag.exception = false;
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                repository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
