using System.Web.Mvc;

namespace IrcBot.Web.Controllers
{
    public class PointsController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
    }
}