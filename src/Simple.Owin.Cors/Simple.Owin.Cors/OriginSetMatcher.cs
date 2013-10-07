namespace Simple.Owin.Cors
{
    using System;
    using System.Collections.Generic;

    public class OriginSetMatcher : IOriginMatcher
    {
        private readonly HashSet<string> _origins;

        public OriginSetMatcher(params string[] origins)
        {
            _origins = new HashSet<string>(origins, StringComparer.InvariantCultureIgnoreCase);
        }

        public OriginSetMatcher(IEnumerable<string> origins)
        {
            _origins = new HashSet<string>(origins, StringComparer.InvariantCultureIgnoreCase);
        }

        public bool IsMatch(string origin)
        {
            return _origins.Contains(origin);
        }
    }

    public static class OriginMatchers
    {
        public static readonly IOriginMatcher[] Wildcard = { new WildcardMatcher() };

        internal class WildcardMatcher : IOriginMatcher
        {
            public bool IsMatch(string origin)
            {
                return true;
            }
        }
    }
}