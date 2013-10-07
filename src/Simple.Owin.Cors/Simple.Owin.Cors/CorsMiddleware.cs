using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simple.Owin.Cors
{
    public static class CorsMiddleware
    {
        public static Func<IDictionary<string, object>, Func<Task>, Task> Build(IEnumerable<IOriginMatcher> origins)
        {
            return new Builder(origins).Build();
        }
    }
}
