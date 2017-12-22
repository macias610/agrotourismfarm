using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using PagedList;
using System.Data.Entity.Validation;
using Microsoft.AspNet.Identity;
using Service.IService;
using DomainModel.Models;
using ViewModel;

namespace AgrotouristicWebApplication.Controllers
{
    public class RoleUsersController : Controller
    {
        private readonly IUserService userService;

        public RoleUsersController(IUserService userService)
        {
            this.userService = userService;
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Index(int? page, bool? concurrencyError)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            List<User> users = userService.GetUsers().ToList();
            Dictionary<string,string> roles = userService.GetRoles().ToDictionary(x=>x.Id,x=>x.Name);
            List<RolesUser> rolesUsers = new List<RolesUser>();
            users.ForEach(user => rolesUsers.Add(
                new RolesUser()
                {
                    Id=user.Id,
                    Email=user.Email,
                    Name=user.Name,
                    Surname=user.Surname,
                    BirthDate=user.BirthDate,
                    Roles= userService.GetUserRoles(user.Id).ToList()
                }
            )
            );
            int currentPage = page ?? 1;
            int perPage = 4;
            if (concurrencyError.GetValueOrDefault())
            {
                ViewBag.ConcurrencyErrorMessage = "Inny użytkownik "
                    + "usunął już ten poziom dostępu lub edytowany użytkownik został usunięty."
                    + "Operacja anulowana. ";
            }
            return View(rolesUsers.ToPagedList<RolesUser>(currentPage,perPage));
        }

        [Authorize(Roles ="Admin")]
        public ActionResult Create(string id, bool? concurrencyError)
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

            List<IdentityUserRole> userRoles = userService.GetUserById(id).Roles.ToList();
            Dictionary<string, string> roles = userService.GetRoles().ToDictionary(x => x.Id, x => x.Name);

            RoleUser roleUser = new RoleUser
            {
                UserId = id,
                Roles = userService.GetNewRolesForUser(userRoles,roles)
            };
            if (concurrencyError.GetValueOrDefault())
            {
                ViewBag.ConcurrencyErrorMessage = "Inny użytkownik "
                    + "dodał maksymalną liczbę poziomów dostępu lub poziom dostepu już został dodany."
                    + "Operacja anulowana. ";
            }
            return View(roleUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Admin")]
        public ActionResult Create([Bind(Include = "UserId,SelectedRoleText")] RoleUser roleUser)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (userService.GetUserById(roleUser.UserId) == null)
                    {
                        return RedirectToAction("Index", new { concurrencyError = true });
                    }
                    else if (userService.GetUserRoles(roleUser.UserId).Count>=2)
                    {
                        return RedirectToAction("Create", new { concurrencyError = true, id = roleUser.UserId });
                    }
                    else if(userService.GetUserRoles(roleUser.UserId).Contains(roleUser.SelectedRoleText))
                    {
                        return RedirectToAction("Create", new { concurrencyError = true, id = roleUser.UserId });
                    }
                    userService.AssignToRole(roleUser.UserId,roleUser.SelectedRoleText);
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

        [Authorize(Roles ="Admin")]
        public ActionResult Delete(string userId,string selectedRoleText, bool? concurrencyError)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            if (selectedRoleText == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (userId == null)
            {
                return HttpNotFound();
            }

            RoleUser roleUser = new RoleUser
            {
                UserId = userId,
                SelectedRoleText = selectedRoleText

            };
            return View(roleUser);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Admin")]
        public ActionResult DeleteConfirmed([Bind(Include = "UserId,SelectedRoleText")] RoleUser roleUser)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            Dictionary<string, string> roles = userService.GetRoles().ToDictionary(x => x.Name, x => x.Id);
            try
            {
                if (roleUser.SelectedRoleText.Equals("Admin") && userService.CountUsersForGivenRole(roles, "Admin") <= 1)
                {
                    ViewBag.error = true;
                    return View(roleUser);
                }
                else if (userService.GetUserById(roleUser.UserId) == null || !userService.GetUserRoles(roleUser.UserId).Contains(roleUser.SelectedRoleText) )
                {
                    return RedirectToAction("Index", new { concurrencyError = true });
                }
                userService.RemoveFromRole(roleUser.UserId, roleUser.SelectedRoleText);
            }
            catch
            {
                ViewBag.exception = true;
                return View(roleUser);
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
