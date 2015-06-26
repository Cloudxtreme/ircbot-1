using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IrcBot.Web.Controllers
{
    public class QuotesController : Controller
    {
        // GET: Quotes
        public ActionResult Index()
        {
            return View();
        }
    }
}