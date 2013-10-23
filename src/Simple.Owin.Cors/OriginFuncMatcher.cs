namespace Simple.Owin.CorsMiddleware
{
    using System;

    /// <summary>
    /// Matches origins using a function
    /// </summary>
    public class OriginFuncMatcher : OriginMatcher
    {
        private readonly Func<string, bool> _test;

        /// <summary>
        /// Initializes a new instance of the <see cref="OriginFuncMatcher"/> class.
        /// </summary>
        /// <param name="test">The test function.</param>
        /// <exception cref="System.ArgumentNullException">test</exception>
        public OriginFuncMatcher(Func<string, bool> test)
        {
            if (test == null) throw new ArgumentNullException("test");
            _test = test;
        }

        /// <summary>
        /// Determines whether the specified origin is a match.
        /// </summary>
        /// <param name="origin">The origin.</param>
        /// <returns>
        ///   The return value of the function supplied to the constructor.
        /// </returns>
        public override bool IsMatch(string origin)
        {
            return _test(origin);
        }
    }
}