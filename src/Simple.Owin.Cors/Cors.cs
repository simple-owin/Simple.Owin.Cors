// ReSharper disable once CheckNamespace
namespace Simple.Owin
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using CorsMiddleware;

    /// <summary>
    /// Static class for starting Cors definitions
    /// </summary>
    public static class Cors
    {
        /// <summary>
        /// Creates a builder with the specified origins.
        /// </summary>
        /// <param name="origins">The origins.</param>
        /// <returns>A <see cref="CorsBuilder"/> that can be built upon using the fluent API</returns>
        /// <remarks>
        /// Thanks to the magic of implicit casting, you can pass multiple types to Create:<br/>
        /// Strings with the full Origin name, e.g. &quot;http://example.com&quot;,<br/>
        /// <see cref="Regex"/> instances for more complex matching, and<br/>
        /// <see cref="Func{T,Result}"/> functions that take a <see cref="string"/> and return a <see cref="bool"/>.
        /// </remarks>
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

        /// <summary>
        /// Creates a builder that allows all origins.
        /// </summary>
        /// <returns>A <see cref="CorsBuilder"/> that can be built upon using the fluent API</returns>
        public static CorsBuilder Wildcard()
        {
            return new CorsBuilder(OriginMatcher.Wildcard);
        }
    }
}
