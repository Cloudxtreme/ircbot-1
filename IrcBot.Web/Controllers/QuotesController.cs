using System.Web.Mvc;

namespace IrcBot.Web.Controllers
{
    [Authorize]
    public class QuotesController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Best()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Worst()
        {
            return View();
        }
    }
}