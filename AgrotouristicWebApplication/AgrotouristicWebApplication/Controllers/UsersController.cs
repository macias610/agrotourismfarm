using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Globalization;
using System.Data.Entity.Infrastructure;
using Microsoft.AspNet.Identity;
using Service.IService;
using DomainModel.Models;

namespace AgrotouristicWebApplication.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }

        [Authorize(Roles ="Admin")]
        public ActionResult Index(int? page)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            IList<User> users = this.userService.GetUsers();
            int currentPage = page ?? 1;
            int perPage = 4;
            return View(users.ToPagedList<User>(currentPage,perPage));
        }

        [Authorize]
        public ActionResult Details(string id)
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
            User user = userService.GetUserById(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            user.isUserEmployed = userService.isUserEmployed(user.Id);
            return View(user);
        }

        [Authorize]
        public ActionResult EditBaseData(string id)
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
            User user = userService.GetUserById(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult EditBaseData(string id, string securityStamp) 
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            securityStamp = Request.Form["EditBaseSecurityStamp"].ToString();
            string[] fieldsToBind = new string[] { "UserName", "Email", "PasswordHash", "SecurityStamp", "PhoneNumber", "Name", "Surname", "BirthDate" };
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User userToUpdate = userService.GetUserById(id);
            if (userToUpdate == null)
            {
                User deletedUser = new User();
                TryUpdateModel(deletedUser, fieldsToBind);
                ModelState.AddModelError(string.Empty,
                    "Nie można zapisać. Użytkownik został usunięty przez innego użytkownika.");
                return View(deletedUser);
            }
            if (TryUpdateModel(userToUpdate, fieldsToBind))
            {
                try
                {
                    userService.UpdateBaseDataUser(userToUpdate, securityStamp);
                    return RedirectToAction("Index","Home");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    User databaseValues = userService.GetOriginalValuesUser(ex.Message);
                    if (databaseValues == null)
                    {
                        ModelState.AddModelError(string.Empty,
                            "Nie można zapisać. Użytkownik został usunięty przez innego użytkownika.");
                    }
                    else
                    {

                        if (databaseValues.UserName != userToUpdate.UserName)
                            ModelState.AddModelError("Nazwa użytkownika", "Aktulalna wartość: "
                                + databaseValues.UserName);
                        if (databaseValues.Email != userToUpdate.Email)
                            ModelState.AddModelError("Email", "Aktulalna wartość: "
                                + databaseValues.Email);
                        if (databaseValues.Name != userToUpdate.Name)
                            ModelState.AddModelError("Imię", "Aktulalna wartość: "
                                + databaseValues.Name);
                        if (databaseValues.Surname != userToUpdate.Surname)
                            ModelState.AddModelError("Nazwisko", "Aktulalna wartość: "
                                + databaseValues.Surname);
                        if (databaseValues.PasswordHash != userToUpdate.PasswordHash)
                            ModelState.AddModelError("Hasło", "Aktulalna wartość: "
                                + databaseValues.PasswordHash);
                        if (databaseValues.PhoneNumber != userToUpdate.PhoneNumber)
                            ModelState.AddModelError("Telefon", "Aktulalna wartość: "
                                + databaseValues.PhoneNumber);
                        if (databaseValues.BirthDate != userToUpdate.BirthDate)
                            ModelState.AddModelError("Data urodzenia", "Aktulalna wartość: "
                                + databaseValues.BirthDate);
                        if (databaseValues.HireDate != userToUpdate.HireDate)
                            ModelState.AddModelError("Data zatrudnienia", "Aktulalna wartość: "
                                + databaseValues.HireDate);
                        if (databaseValues.Profession != userToUpdate.Profession)
                            ModelState.AddModelError("Profesja", "Aktulalna wartość: "
                                + databaseValues.Profession);
                        if (databaseValues.Salary != userToUpdate.Salary)
                            ModelState.AddModelError("Pensja", "Aktulalna wartość: "
                                + String.Format("{0:c}", databaseValues.Salary));
                        ModelState.AddModelError(string.Empty, "Edytowany rekord  "
                            + "został wcześniej zmodyfikowany przez innego użytkownika,operacja anulowana. Pobrano wpisane wartości. Kliknij zapisz, żeby napisać.");
                        userToUpdate.SecurityStamp = databaseValues.SecurityStamp;
                    }
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Nie można zapisać. Spróbuj ponownie");
                }
            }
            return View(userToUpdate);
        }

        [Authorize(Roles ="Admin")]
        public ActionResult Edit(string id)
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
            User user = userService.GetUserById(id);
            user.isUserEmployed = userService.isUserEmployed(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            IList<string> professions = userService.GetAvaiableProfessons();
            ViewData["Professions"] = professions;
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Admin")]
        public ActionResult Edit(string id,string securityStamp)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            securityStamp = Request.Form["EditSecurityStamp"].ToString();
            string[] fieldsToBind = new string[] { "UserName", "Email","PasswordHash","SecurityStamp","PhoneNumber","Name","Surname","BirthDate","HireDate","Profession","Salary" };
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User userToUpdate = userService.GetUserById(id);
            if (userToUpdate == null)
            {
                User deletedUser = new User();
                TryUpdateModel(deletedUser, fieldsToBind);
                ModelState.AddModelError(string.Empty,
                    "Nie można zapisać. Użytkownik został usunięty przez innego użytkownika.");
                return View(deletedUser);
            }
            if (TryUpdateModel(userToUpdate, fieldsToBind))
            {
                try
                {
                    userService.UpdateUser(userToUpdate, securityStamp);
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    User databaseValues = userService.GetOriginalValuesUser(ex.Message);
                    if (databaseValues == null)
                    {
                        ModelState.AddModelError(string.Empty,
                            "Nie można zapisać. Użytkownik został usunięty przez innego użytkownika.");
                    }
                    else
                    {

                        if (databaseValues.UserName != userToUpdate.UserName)
                            ModelState.AddModelError("Nazwa użytkownika", "Aktulalna wartość: "
                                + databaseValues.UserName);
                        if (databaseValues.Email != userToUpdate.Email)
                            ModelState.AddModelError("Email", "Aktulalna wartość: "
                                + databaseValues.Email);
                        if (databaseValues.Name != userToUpdate.Name)
                            ModelState.AddModelError("Imię", "Aktulalna wartość: "
                                + databaseValues.Name);
                        if (databaseValues.Surname != userToUpdate.Surname)
                            ModelState.AddModelError("Nazwisko", "Aktulalna wartość: "
                                + databaseValues.Surname);
                        if (databaseValues.PasswordHash != userToUpdate.PasswordHash)
                            ModelState.AddModelError("Hasło", "Aktulalna wartość: "
                                + databaseValues.PasswordHash);
                        if (databaseValues.PhoneNumber != userToUpdate.PhoneNumber)
                            ModelState.AddModelError("Telefon", "Aktulalna wartość: "
                                + databaseValues.PhoneNumber);
                        if (databaseValues.BirthDate != userToUpdate.BirthDate)
                            ModelState.AddModelError("Data urodzenia", "Aktulalna wartość: "
                                + databaseValues.BirthDate);
                        if (databaseValues.HireDate != userToUpdate.HireDate)
                            ModelState.AddModelError("Data zatrudnienia", "Aktulalna wartość: "
                                + databaseValues.HireDate);
                        if (databaseValues.Profession != userToUpdate.Profession)
                            ModelState.AddModelError("Profesja", "Aktulalna wartość: "
                                + databaseValues.Profession);
                        if (databaseValues.Salary != userToUpdate.Salary)
                            ModelState.AddModelError("Pensja", "Aktulalna wartość: "
                                + String.Format("{0:c}", databaseValues.Salary));
                        ModelState.AddModelError(string.Empty, "Edytowany rekord  "
                            + "został wcześniej zmodyfikowany przez innego użytkownika,operacja anulowana. Pobrano wpisane wartości. Kliknij zapisz, żeby napisać.");
                        userToUpdate.SecurityStamp = databaseValues.SecurityStamp;
                    }
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Nie można zapisać. Spróbuj ponownie");
                }
            }
            userToUpdate.isUserEmployed = userService.isUserEmployed(id);
            IList<string> professions = userService.GetAvaiableProfessons();
            ViewData["Professions"] = professions;
            return View(userToUpdate);
        }

        [Authorize(Roles ="Admin")]
        public ActionResult Delete(string id,bool? concurrencyError)
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
            User user = userService.GetUserById(id);
            if (user == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("Index");
                }
                return HttpNotFound();
            }
            if (concurrencyError.GetValueOrDefault())
            {
                ViewBag.ConcurrencyErrorMessage = "Usuwany rekord "
                    + "został zmodyfikowany po pobraniu oryginalnych wartości."
                    + "Usuwanie anulowano i wyświetlono aktualne dane. "
                    + "W celu usunięcia kliknij usuń";
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Admin")]
        public ActionResult DeleteConfirmed([Bind(Include = "Id,Name,Surname,Email,BirthDate,SecurityStamp")]User user)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            Dictionary<string, string> roles = userService.GetRoles().ToDictionary(x => x.Name, x => x.Id);
            Dictionary<string, string> rolesSecond = userService.GetRoles().ToDictionary(x => x.Id, x => x.Name);
            if (userService.GetUserRoles(user.Id).Contains("Admin") && userService.GetNumberOfUsersForGivenRole(roles, "Admin") <= 1)
            {
                ViewBag.error = true;
                return View(user);
            }
            string securityStamp = Request.Form["DeleteSecurityStamp"].ToString();
            try
            {
                if (userService.GetUserRoles(user.Id).Contains("Klient"))
                    userService.RemoveReservationsAssosiatedClient(user.Id);
                userService.RemoveUser(user, securityStamp);
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction("Delete", new { concurrencyError = true, id = user.Id });
            }
            catch (DataException ex)
            {
                ModelState.AddModelError(string.Empty, "Nie można usunąć.Spróbuj ponownie.");
                return View(user);
            }
        }

        protected override void Dispose(bool disposing)
        { 
            base.Dispose(disposing);
        }
    }
}
