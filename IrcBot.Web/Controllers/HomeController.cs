using System;
using System.Web.Mvc;

using IrcBot.Entities.Dto;

namespace IrcBot.Web.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginAttemptDto loginAttempt, string returnUrl)
        {
            if (String.IsNullOrEmpty(loginAttempt.Email) || String.IsNullOrWhiteSpace(loginAttempt.Email))
            {
                ModelState.AddModelError("", "Email address is required");
                return View();
            }

            if (String.IsNullOrEmpty(loginAttempt.Password) || String.IsNullOrWhiteSpace(loginAttempt.Password))
            {
                ModelState.AddModelError("", "Password is required");
                return View();
            }



            return RedirectToAction("index", "dashboard");
        }
    }
}