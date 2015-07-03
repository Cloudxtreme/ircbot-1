using System.Web.Mvc;

namespace IrcBot.Web.Controllers
{
    [Authorize]
    public class PreferencesController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
    }
}