﻿using System.Web.Mvc;

namespace IrcBot.Web.Controllers
{
    public class MessagesController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Realtime()
        {
            return View();
        }
    }
}