namespace Simple.Owin.CorsMiddleware
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    internal static class OwinHelpers
    {
        public static string GetRequestHeaderValue(IDictionary<string, object> env, string key)
        {
            var headers = env.GetValueOrDefault<IDictionary<string, string[]>>(OwinKeys.RequestHeaders);
            if (headers == null) return null;
            string[] values;
            return !headers.TryGetValue(key, out values) ? null : values.FirstOrDefault();
        }

        public static T GetValueOrDefault<T>(this IDictionary<string, object> env, string key)
        {
            object obj;
            if (env.TryGetValue(key, out obj))
            {
                if (obj is T)
                {
                    return (T) obj;
                }
            }

            return default(T);
        }

        public static void SetResponseHeaderValue(IDictionary<string, object> env, string key, string value)
        {
            var headers = env.GetValueOrDefault<IDictionary<string, string[]>>(OwinKeys.ResponseHeaders);
            if (headers == null)
            {
                headers = new Dictionary<string, string[]>();
                env[OwinKeys.ResponseHeaders] = headers;
            }

            headers[key] = new[] {value};
        }

        public static Task Stop(IDictionary<string, object> env, int statusCode)
        {
            env[OwinKeys.StatusCode] = statusCode;
            var tcs = new TaskCompletionSource<int>();
            tcs.SetResult(0);
            return tcs.Task;
        }
    }
}