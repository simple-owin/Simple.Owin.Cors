namespace Simple.Owin.Cors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class OriginSetMatcher : OriginMatcher
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

        public override bool IsMatch(string origin)
        {
            return _origins.Contains(origin);
        }

        public static OriginSetMatcher Combine(List<OriginSetMatcher> sets)
        {
            return new OriginSetMatcher(sets.SelectMany(s => s._origins));
        }
    }

    public class OriginFuncMatcher : OriginMatcher
    {
        private readonly Func<string, bool> _test;

        public OriginFuncMatcher(Func<string, bool> test)
        {
            if (test == null) throw new ArgumentNullException("test");
            _test = test;
        }

        public override bool IsMatch(string origin)
        {
            return _test(origin);
        }
    }
}