namespace Simple.Owin.CorsMiddleware
{
    using System;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Matches origins using a regular expression.
    /// </summary>
    public class OriginRegexMatcher : OriginMatcher
    {
        private readonly Regex _regex;

        /// <summary>
        /// Initializes a new instance of the <see cref="OriginRegexMatcher"/> class.
        /// </summary>
        /// <param name="pattern">The regular expression pattern.</param>
        /// <exception cref="ArgumentNullException">pattern</exception>
        public OriginRegexMatcher(string pattern)
        {
            if (pattern == null) throw new ArgumentNullException("pattern");
            _regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OriginRegexMatcher"/> class.
        /// </summary>
        /// <param name="regex">The <see cref="Regex"/>.</param>
        /// <exception cref="ArgumentNullException">regex</exception>
        public OriginRegexMatcher(Regex regex)
        {
            if (regex == null) throw new ArgumentNullException("regex");
            _regex = regex;
        }

        /// <summary>
        /// Determines whether the specified origin is a match.
        /// </summary>
        /// <param name="origin">The origin.</param>
        /// <returns>
        ///   <c>true</c> if the origin matches the regular expression; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsMatch(string origin)
        {
            return _regex.IsMatch(origin);
        }
    }
}