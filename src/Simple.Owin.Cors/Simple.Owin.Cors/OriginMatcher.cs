namespace Simple.Owin.Cors
{
    using System;
    using System.Linq.Expressions;
    using System.Text.RegularExpressions;

    public abstract class OriginMatcher
    {
        public abstract bool IsMatch(string origin);

        public static readonly OriginMatcher Wildcard = new WildcardMatcher();

        internal class WildcardMatcher : OriginMatcher
        {
            public override bool IsMatch(string origin)
            {
                return true;
            }
        }

        public static implicit operator OriginMatcher(string origin)
        {
            if (origin.StartsWith("/"))
            {
                return new OriginRegexMatcher(origin.Trim('/'));
            }
            return new OriginSetMatcher(origin);
        }

        public static implicit operator OriginMatcher(Regex pattern)
        {
            return new OriginRegexMatcher(pattern);
        }

        public static implicit operator OriginMatcher(Func<string, bool> test)
        {
            return new OriginFuncMatcher(test);
        }
        
        public static implicit operator OriginMatcher(Expression<Func<string, bool>> test)
        {
            return new OriginFuncMatcher(test.Compile());
        }
    }
}