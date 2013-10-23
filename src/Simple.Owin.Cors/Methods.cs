namespace Simple.Owin.CorsMiddleware
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;

    internal class Methods
    {
        public static readonly MethodInfo GetRequestHeaderValue =
            MethodOf<IDictionary<string, object>, string, string>(OwinHelpers.GetRequestHeaderValue);
        public static readonly MethodInfo SetResponseHeaderValue =
            MethodOf<IDictionary<string, object>, string, string>(OwinHelpers.SetResponseHeaderValue);

        public static readonly MethodInfo MirrorRequestMethods =
            MethodOf<IDictionary<string, object>>(OwinHelpers.MirrorRequestMethods);
        public static readonly MethodInfo MirrorRequestHeaders =
            MethodOf<IDictionary<string, object>>(OwinHelpers.MirrorRequestHeaders);

        public static readonly MethodInfo StringIsNullOrWhitespace = 
            MethodOf<string,bool>(string.IsNullOrWhiteSpace);

        public static readonly MethodInfo Stop = MethodOf<IDictionary<string,object>, int, Task>(OwinHelpers.Stop);

        private static MethodInfo MethodOf<T1, T>(Func<T1, T> func)
        {
            return func.Method;
        }

        private static MethodInfo MethodOf<T>(Action<T> func)
        {
            return func.Method;
        }

        private static MethodInfo MethodOf<T>(Func<T> func)
        {
            return func.Method;
        }

        private static MethodInfo MethodOf<T1, T2, T>(Func<T1, T2, T> func)
        {
            return func.Method;
        }

        private static MethodInfo MethodOf<T1, T2, T3>(Action<T1, T2, T3> action)
        {
            return action.Method;
        }
    }
}