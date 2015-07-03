using System.Web.Mvc;

namespace IrcBot.Web.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Users()
        {
            return View();
        }
    }
}