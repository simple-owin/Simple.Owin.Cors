using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simple.Owin.Cors
{
    using System.Linq;

    public static class CorsMiddleware
    {
        public static CorsBuilder Create(params OriginMatcher[] origins)
        {
            var sets = origins.OfType<OriginSetMatcher>().ToList();
            var list = origins.Except(sets).ToList();

            var setMatcher = OriginSetMatcher.Combine(sets);
            return new CorsBuilder(origins);
        }

        public static CorsBuilder Wildcard()
        {
            return new CorsBuilder(OriginMatcher.Wildcard);
        }
    }
}
