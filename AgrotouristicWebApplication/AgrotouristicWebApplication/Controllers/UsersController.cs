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
using Repository.Repo;
using PagedList;
using System.Globalization;
using System.Data.Entity.Infrastructure;

namespace AgrotouristicWebApplication.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserRepository repository;

        public UsersController(IUserRepository repository)
        {
            this.repository = repository;
        }

        [Authorize(Roles ="Admin")]
        public ActionResult Index(int? page)
        {
            List<User> users = repository.GetUsers().ToList();
            int currentPage = page ?? 1;
            int perPage = 4;
            return View(users.ToPagedList<User>(currentPage,perPage));
        }

        [Authorize]
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
            user.isUserEmployed = repository.isUserEmployed(user.Id);
            return View(user);
        }

        [Authorize]
        public ActionResult EditBaseData(string id)
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

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult EditBaseData(string id, string securityStamp) 
        {
            securityStamp = Request.Form["EditBaseSecurityStamp"].ToString();
            string[] fieldsToBind = new string[] { "UserName", "Email", "PasswordHash", "SecurityStamp", "PhoneNumber", "Name", "Surname", "BirthDate" };
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User userToUpdate = repository.GetUserById(id);
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
                    repository.UpdateBaseDataUser(userToUpdate, securityStamp);
                    repository.SaveChanges();
                    return RedirectToAction("Index","Home");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    User databaseValues = repository.GetOriginalValuesUser(ex.Message);
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
            IList<string> professions = repository.GetAvaiableProfessons();
            ViewData["Professions"] = professions;
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Admin")]
        public ActionResult Edit(string id,string securityStamp)
        {
            securityStamp = Request.Form["EditSecurityStamp"].ToString();
            string[] fieldsToBind = new string[] { "UserName", "Email","PasswordHash","SecurityStamp","PhoneNumber","Name","Surname","BirthDate","HireDate","Profession","Salary" };
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User userToUpdate = repository.GetUserById(id);
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
                    repository.UpdateUser(userToUpdate, securityStamp);
                    repository.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    User databaseValues = repository.GetOriginalValuesUser(ex.Message);
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
            userToUpdate.isUserEmployed = repository.isUserEmployed(id);
            IList<string> professions = repository.GetAvaiableProfessons();
            ViewData["Professions"] = professions;
            return View(userToUpdate);
        }

        [Authorize(Roles ="Admin")]
        public ActionResult Delete(string id,bool? concurrencyError)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = repository.GetUserById(id);
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
            Dictionary<string, string> roles = repository.GetRoles().ToDictionary(x => x.Name, x => x.Id);
            Dictionary<string, string> rolesSecond = repository.GetRoles().ToDictionary(x => x.Id, x => x.Name);
            if (repository.GetUserRoles(user.Id).Contains("Admin") && repository.GetNumberOfUsersForGivenRole(roles, "Admin") <= 1)
            {
                ViewBag.error = true;
                return View(user);
            }
            string securityStamp = Request.Form["DeleteSecurityStamp"].ToString();
            try
            {
                repository.RemoveUser(user, securityStamp);
                repository.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction("Delete", new { concurrencyError = true, id = user.Id });
            }
            catch (DataException)
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
