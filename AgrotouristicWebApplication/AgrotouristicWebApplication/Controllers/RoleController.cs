using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Service.IService;
using Service.Service;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AgrotouristicWebApplication.Controllers
{
    public class RoleController : Controller
    {
        private readonly IUserService userService;

        public RoleController(IUserService userService)
        {
            this.userService = userService;
        }

        [Authorize(Roles ="Admin")]
        public ActionResult Index()
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            IList<IdentityRole> roles = userService.GetRoles();
            return View(roles);
        }

        public ActionResult Create()
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            return View();
        }

        [HttpPost]
        [Authorize(Roles ="Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include ="Name")] IdentityRole role)
        {
            if (HttpContext.Session["Checker"] == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home", new { expiredSession = true });
            }
            if (ModelState.IsValid)
            {
                List<IdentityRole> roles = userService.GetRoles().ToList();
                if (roles.Contains(role,new RoleComparer()))
                {
                    ViewBag.error = true;
                    ModelState.Clear();
                    return View();
                }

                try
                {
                    userService.AddRole(role);
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
