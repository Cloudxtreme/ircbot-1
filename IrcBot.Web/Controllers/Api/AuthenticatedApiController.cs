using System.Linq;
using System.Net;
using System.Web.Http;

using IrcBot.Common.Infrastructure;
using IrcBot.Database.UnitOfWork;
using IrcBot.Entities.Models;
using IrcBot.Service;

namespace IrcBot.Web.Controllers.Api
{
    public class AuthenticatedApiController : ApiController
    {
        protected readonly IUnitOfWorkAsync UnitOfWork;
        protected readonly IUserService UserService;

        protected User CurrentUser;

        public AuthenticatedApiController(IUnitOfWorkAsync unitOfWork, IUserService userService)
        {
            UnitOfWork = unitOfWork;
            UserService = userService;

            if (!HttpContextFactory.Current.Request.IsAuthenticated)
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            var user = UserService.Query(x =>
                x.Email == HttpContextFactory.Current.User.Identity.Name)
                .Select()
                .SingleOrDefault();

            if (user == null)
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            CurrentUser = user;
        }
    }
}