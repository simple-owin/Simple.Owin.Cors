namespace Simple.Owin.CorsMiddleware
{
    internal static class HeaderKeys
    {
        /// <summary>
        /// The <c>Access-Control-Allow-Origin</c> response header field.
        /// </summary>
        public const string AccessControlAllowOrigin = "Access-Control-Allow-Origin";

        /// <summary>
        /// The <c>Access-Control-Allow-Methods</c> response header field.
        /// </summary>
        public const string AccessControlAllowMethods = "Access-Control-Allow-Methods";

        /// <summary>
        /// The <c>Access-Control-Allow-Headers</c> response header field.
        /// </summary>
        public const string AccessControlAllowHeaders = "Access-Control-Allow-Headers";

        /// <summary>
        /// The <c>Access-Control-Expose-Headers</c> response header field.
        /// </summary>
        public const string AccessControlExposeHeaders = "Access-Control-Expose-Headers";

        /// <summary>
        /// The <c>Access-Control-Max-Age</c> response header field.
        /// </summary>
        public const string AccessControlMaxAge = "Access-Control-Max-Age";

        /// <summary>
        /// The <c>Access-Control-Allow-Credentials</c> response header field.
        /// </summary>
        public const string AccessControlAllowCredentials = "Access-Control-Allow-Credentials";
    }
}