using System.Web.Mvc;
using System.Web.Security;

using IrcBot.Common.Infrastructure;

namespace IrcBot.Web.Controllers
{
    public class LogoutController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            if (HttpContextFactory.Current.Request.IsAuthenticated)
            {
                FormsAuthentication.SignOut();
            }

            return RedirectToAction("index", "home");
        }
    }
}