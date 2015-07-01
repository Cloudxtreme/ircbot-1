using System;
using System.Web;

namespace IrcBot.Common.Infrastructure
{
    public sealed class HttpContextFactory
    {
        private static HttpContextBase _context;

        public static HttpContextBase Current
        {
            get
            {
                if (_context != null)
                {
                    return _context;
                }

                if (HttpContext.Current == null)
                {
                    throw new InvalidOperationException("HttpContext is not available");
                }

                return new HttpContextWrapper(HttpContext.Current);
            }
        }

        public static void SetCurrentContext(HttpContextBase httpContext)
        {
            _context = httpContext;
        }
    }
}
