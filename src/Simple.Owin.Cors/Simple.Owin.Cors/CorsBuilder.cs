namespace Simple.Owin.CorsMiddleware
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

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

        public CorsBuilder AllowCredentials()
        {
            return new CorsBuilder(this, allowCredentials: true);
        }

        public CorsBuilder AllowMethods(params string[] methods)
        {
            return new CorsBuilder(this, allowMethods: methods);
        }

        public CorsBuilder AllowHeaders(params string[] headers)
        {
            return new CorsBuilder(this, allowHeaders: headers);
        }

        public CorsBuilder ExposeHeaders(params string[] headers)
        {
            return new CorsBuilder(this, exposeHeaders: headers);
        }

        public CorsBuilder MaxAge(long seconds)
        {
            return new CorsBuilder(this, maxAge: seconds);
        }

        public CorsBuilder MaxAge(TimeSpan timeSpan)
        {
            return MaxAge((long)timeSpan.TotalSeconds);
        }

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

        public static implicit operator Func<IDictionary<string, object>, Func<IDictionary<string, object>, Task>, Task>(CorsBuilder builder)
        {
            return builder.Build();
        }

        public CorsBuilder UseStopStatus(int stopStatus)
        {
            return new CorsBuilder(this, stopStatus: stopStatus);
        }
    }
}