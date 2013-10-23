namespace Simple.Owin.CorsMiddleware
{
    using System;
    using System.Linq.Expressions;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The base class for Origin Matchers.
    /// </summary>
    public abstract class OriginMatcher
    {
        /// <summary>
        /// Determines whether the specified origin is a match.
        /// </summary>
        /// <param name="origin">The origin.</param>
        /// <returns><c>true</c> if the origin matches the rules; otherwise, <c>false</c>.</returns>
        public abstract bool IsMatch(string origin);

        /// <summary>
        /// The wildcard matcher.
        /// </summary>
        public static readonly OriginMatcher Wildcard = new WildcardMatcher();

        internal class WildcardMatcher : OriginMatcher
        {
            public override bool IsMatch(string origin)
            {
                return true;
            }
        }

        /// <summary>
        /// Implicit cast from <see cref="string"/>.
        /// </summary>
        /// <param name="origin">The origin.</param>
        /// <returns>If the string starts with <c>/</c>, an <see cref="OriginRegexMatcher"/>; otherwise, an <see cref="OriginSetMatcher"/>.</returns>
        public static implicit operator OriginMatcher(string origin)
        {
            if (origin.StartsWith("/"))
            {
                return new OriginRegexMatcher(origin.Trim('/'));
            }
            return new OriginSetMatcher(origin);
        }

        /// <summary>
        /// Implicit cast from <see cref="Regex"/>
        /// </summary>
        /// <param name="regex">The regex.</param>
        /// <returns>An <see cref="OriginRegexMatcher"/>.</returns>
        public static implicit operator OriginMatcher(Regex regex)
        {
            return new OriginRegexMatcher(regex);
        }

        /// <summary>
        /// Implicit cast from <see cref="Func{T,TResult}"/> of <see cref="string"/>, <see cref="bool"/>.
        /// </summary>
        /// <param name="test">The test function.</param>
        /// <returns>An <see cref="OriginFuncMatcher"/>.</returns>
        public static implicit operator OriginMatcher(Func<string, bool> test)
        {
            return new OriginFuncMatcher(test);
        }
    }
}