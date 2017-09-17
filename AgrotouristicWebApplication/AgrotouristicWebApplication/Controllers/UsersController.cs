﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Repository.Models;
using Repository.IRepo;
using Repository.Repo;
using PagedList;

namespace AgrotouristicWebApplication.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserRepository repository;

        public UsersController(IUserRepository repository)
        {
            this.repository = repository;
        }

        // GET: Users
        [Authorize(Roles ="Admin")]
        public ActionResult Index(int? page)
        {
            List<User> users = repository.GetUsers().ToList();
            int currentPage = page ?? 1;
            int perPage = 4;
            return View(users.ToPagedList<User>(currentPage,perPage));
        }

        [Authorize(Roles ="Admin")]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = repository.GetUserById(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [Authorize(Roles ="Admin")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = repository.GetUserById(id);
            user.isUserEmployed = repository.isUserEmployed(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Admin")]
        public ActionResult Edit([Bind(Include = "Id,Email,PasswordHash,SecurityStamp,PhoneNumber,Name,Surname,BirthDate,HireDate,Profession,Salary")] User user)
        {
            if (ModelState.IsValid)
            {
                user.UserName = user.Email;
                try
                {
                    repository.UpdateUser(user);
                    repository.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch 
                {
                    ViewBag.exception = false;
                    return View(user);
                }
               
            }
            return View(user);
        }

        [Authorize(Roles ="Admin")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = repository.GetUserById(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Admin")]
        public ActionResult DeleteConfirmed(string id)
        {
            Dictionary<string, string> roles = repository.GetRoles().ToDictionary(x => x.Name, x => x.Id);
            Dictionary<string, string> rolesSecond = repository.GetRoles().ToDictionary(x => x.Id, x => x.Name);
            List<String> userRoles = new List<string>();
            User user = repository.GetUserById(id);

            user.Roles.ToList().ForEach(item => userRoles.Add(rolesSecond[item.RoleId]));

            if (userRoles.Contains("Admin") && repository.GetNumberOfUsersForGivenRole(roles, "Admin") <= 1)
            {
                ViewBag.error = true;
                return View(user);
            }

            try
            {
                repository.RemoveUser(user.Id);
                repository.SaveChanges();
            }
            catch
            {
                ViewBag.exception = true;
                return View(user);
            }
            ViewBag.error = false;
            ViewBag.exception = false;
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        { 
            base.Dispose(disposing);
        }
    }
}
