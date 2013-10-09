// ReSharper disable once CheckNamespace
namespace Simple.Owin
{
    using System.Linq;
    using CorsMiddleware;

    public static class Cors
    {
        public static CorsBuilder Create(params OriginMatcher[] origins)
        {
            var sets = origins.OfType<OriginSetMatcher>().ToList();
            if (sets.Count == 0)
            {
                return new CorsBuilder(origins);
            }
            var list = origins.Except(sets).ToList();

            var setMatcher = OriginSetMatcher.Combine(sets);
            list.Insert(0, setMatcher);
            return new CorsBuilder(list.ToArray());
        }

        public static CorsBuilder Wildcard()
        {
            return new CorsBuilder(OriginMatcher.Wildcard);
        }
    }
}
