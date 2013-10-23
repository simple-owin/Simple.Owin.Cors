namespace Simple.Owin.CorsMiddleware
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Matches <c>Host</c> against a set of allowed origins.
    /// </summary>
    public class OriginSetMatcher : OriginMatcher
    {
        private readonly HashSet<string> _origins;

        /// <summary>
        /// Initializes a new instance of the <see cref="OriginSetMatcher"/> class.
        /// </summary>
        /// <param name="origins">The allowed origins.</param>
        public OriginSetMatcher(params string[] origins)
        {
            _origins = new HashSet<string>(origins, StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OriginSetMatcher"/> class.
        /// </summary>
        /// <param name="origins">The allowed origins.</param>
        public OriginSetMatcher(IEnumerable<string> origins)
        {
            _origins = new HashSet<string>(origins, StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Determines whether the specified origin is a match.
        /// </summary>
        /// <param name="origin">The origin.</param>
        /// <returns>
        ///   <c>true</c> if the origin is in the set; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsMatch(string origin)
        {
            return _origins.Contains(origin);
        }

        /// <summary>
        /// Combines multiple instances into a single instance.
        /// </summary>
        /// <param name="sets">The sets.</param>
        /// <returns>A single <see cref="OriginSetMatcher"/> whose set is the union of the passed instance's sets.</returns>
        public static OriginSetMatcher Combine(List<OriginSetMatcher> sets)
        {
            return new OriginSetMatcher(sets.SelectMany(s => s._origins));
        }
    }
}