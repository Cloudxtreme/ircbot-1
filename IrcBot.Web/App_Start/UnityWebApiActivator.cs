using System.Web.Http;

using Microsoft.Practices.Unity.WebApi;

using IrcBot.Web;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(UnityWebApiActivator), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(UnityWebApiActivator), "Shutdown")]

namespace IrcBot.Web
{
    public static class UnityWebApiActivator
    {
        public static void Start() 
        {
            var resolver = new UnityDependencyResolver(UnityConfig.GetConfiguredContainer());

            GlobalConfiguration.Configuration.DependencyResolver = resolver;
        }

        public static void Shutdown()
        {
            var container = UnityConfig.GetConfiguredContainer();
            container.Dispose();
        }
    }
}
