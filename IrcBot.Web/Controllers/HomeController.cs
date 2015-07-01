﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;

using IrcBot.Common.Encryption;
using IrcBot.Entities.Dto;
using IrcBot.Service;

namespace IrcBot.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserService _userService;

        public HomeController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <ActionResult> Index(LoginAttemptDto loginAttempt, string returnUrl)
        {
            if (String.IsNullOrEmpty(loginAttempt.Email) || String.IsNullOrWhiteSpace(loginAttempt.Email))
            {
                //ModelState.AddModelError("", "Email address is required");
                return View();
            }

            if (String.IsNullOrEmpty(loginAttempt.Password) || String.IsNullOrWhiteSpace(loginAttempt.Password))
            {
                //ModelState.AddModelError("", "Password is required");
                return View();
            }

            var encryptedPassword = PasswordEncryption.Encrypt(loginAttempt.Password);

            var user = (await _userService.Query(x =>
                x.Email == loginAttempt.Email &&
                x.Password == encryptedPassword).SelectAsync()).SingleOrDefault();

            if (user != null)
            {
                FormsAuthentication.SetAuthCookie(user.Email, true);
                return RedirectToAction("index", "dashboard");
            }

            return View();
        }
    }
}