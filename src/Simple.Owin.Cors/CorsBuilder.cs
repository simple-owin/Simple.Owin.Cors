namespace Simple.Owin.CorsMiddleware
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// The builder class to enable the fluent API.
    /// </summary>
    public sealed class CorsBuilder
    {
        private readonly IEnumerable<OriginMatcher> _matchers;
        private readonly bool? _allowCredentials;
        private readonly string[] _allowMethods;
        private readonly string[] _allowHeaders;
        private readonly string[] _exposeHeaders;
        private readonly double _maxAge;
        private readonly int _stopStatus;

        internal CorsBuilder(params OriginMatcher[] matchers)
        {
            _matchers = matchers;
            _stopStatus = 403;
        }

        private CorsBuilder(CorsBuilder copy, bool? allowCredentials = null, string[] allowMethods = null, string[] allowHeaders = null, string[] exposeHeaders = null, double? maxAge = null, int? stopStatus = null)
        {
            _matchers = copy._matchers;
            _allowCredentials = allowCredentials ?? copy._allowCredentials;
            _allowMethods = allowMethods ?? copy._allowMethods;
            _allowHeaders = allowHeaders ?? copy._allowHeaders;
            _exposeHeaders = exposeHeaders ?? copy._exposeHeaders;
            _maxAge = maxAge ?? copy._maxAge;
            _stopStatus = stopStatus ?? copy._stopStatus;
        }

        /// <summary>
        /// Sets a flag to allow credentials on requests.
        /// </summary>
        /// <returns>The <see cref="CorsBuilder"/> instance.</returns>
        public CorsBuilder AllowCredentials()
        {
            return new CorsBuilder(this, allowCredentials: true);
        }

        /// <summary>
        /// Sets the methods (e.g. &quot;GET&quot;, &quot;POST&quot;, etc.) allowed on requests.
        /// </summary>
        /// <param name="methods">The methods.</param>
        /// <returns>The <see cref="CorsBuilder"/> instance.</returns>
        /// <remarks>Although the HTTP spec does not allow wild-cards for methods, this library does. Where a wildcard is used,
        /// the value of the <c>Access-Control-Request-Methods</c> request header will be copied into the
        /// <c>Access-Control-Allow-Methods</c> response header.</remarks>
        public CorsBuilder AllowMethods(params string[] methods)
        {
            return new CorsBuilder(this, allowMethods: methods);
        }

        /// <summary>
        /// Sets the request header names allowed on requests.
        /// </summary>
        /// <param name="headers">The header names.</param>
        /// <returns>The <see cref="CorsBuilder"/> instance.</returns>
        /// <remarks>Although the HTTP spec does not allow wild-cards for headers, this library does. Where a wildcard is used,
        /// the value of the <c>Access-Control-Request-Headers</c> request header will be copied into the
        /// <c>Access-Control-Allow-Headers</c> response header.</remarks>
        public CorsBuilder AllowHeaders(params string[] headers)
        {
            return new CorsBuilder(this, allowHeaders: headers);
        }

        /// <summary>
        /// Sets the response header names that it is OK for the client to use.
        /// </summary>
        /// <param name="headers">The header names.</param>
        /// <returns>The <see cref="CorsBuilder"/> instance.</returns>
        /// <remarks>This method does not allow wildcards.</remarks>
        public CorsBuilder ExposeHeaders(params string[] headers)
        {
            return new CorsBuilder(this, exposeHeaders: headers);
        }

        /// <summary>
        /// Sets the maximum length of time, in seconds, that the result of a pre-flighted request can be cached.
        /// </summary>
        /// <param name="seconds">The number of seconds the result can be cached.</param>
        /// <returns>The <see cref="CorsBuilder"/> instance.</returns>
        public CorsBuilder MaxAge(long seconds)
        {
            return new CorsBuilder(this, maxAge: seconds);
        }

        /// <summary>
        /// Sets the maximum length of time that the result of a pre-flighted request can be cached.
        /// </summary>
        /// <param name="timeSpan">The time the result can be cached.</param>
        /// <returns>The <see cref="CorsBuilder"/> instance.</returns>
        public CorsBuilder MaxAge(TimeSpan timeSpan)
        {
            return MaxAge((long)timeSpan.TotalSeconds);
        }

        /// <summary>
        /// Sets a custom HTTP status code to use when halting an invalid CORS request.
        /// </summary>
        /// <param name="stopStatus">The stop status. Defaults to 403 (Forbidden).</param>
        /// <returns>The <see cref="CorsBuilder"/> instance.</returns>
        public CorsBuilder UseStopStatus(int stopStatus)
        {
            return new CorsBuilder(this, stopStatus: stopStatus);
        }

        /// <summary>
        /// Builds the OWIN AppFunc delegate for the module.
        /// </summary>
        /// <returns>The AppFunc delegate.</returns>
        public Func<IDictionary<string, object>, Func<IDictionary<string, object>, Task>, Task> Build()
        {
            var builder = new Builder(_matchers)
            {
                AllowCredentials = _allowCredentials,
                AllowMethods = _allowMethods,
                AllowHeaders = _allowHeaders,
                ExposeHeaders = _exposeHeaders,
                MaxAge = _maxAge,
                StopStatus = _stopStatus
            };

            return builder.Build();
        }

        /// <summary>
        /// Implicit cast to the AppFunc delegate.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>The AppFunc delegate.</returns>
        public static implicit operator Func<IDictionary<string, object>, Func<IDictionary<string, object>, Task>, Task>(CorsBuilder builder)
        {
            return builder.Build();
        }
    }
}