using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Owin.CorsMiddleware.Tests
{
    using Xunit;
    using AppFunc = Func<IDictionary<string,object>, Task>;

    public class CheckTests
    {
        [Fact]
        public void ItCallsTheNextFuncWithWildcard()
        {
            var func = Cors.Create(OriginMatcher.Wildcard).Build();
            var env = new Dictionary<string, object>();
            bool pass = false;
            AppFunc next = e =>
            {
                pass = true;
                return Completed(e);
            };
            func(env, next);
            Assert.True(pass);
        }

        [Fact]
        public void ItCallsTheNextFuncWithSet()
        {
            var func = Cors.Create(new OriginSetMatcher("https://cors.com")).Build();
            var env = CreateEnv();
            bool pass = false;
            AppFunc next = e =>
            {
                pass = true;
                return Completed(e);
            };
            func(env, next);
            Assert.True(pass);
        }

        [Fact]
        public void ItDoesNotCallTheNextFuncWithSet()
        {
            var func = Cors.Create(new OriginSetMatcher("https://reject.com")).Build();
            var env = CreateEnv();
            bool pass = false;
            AppFunc next = e =>
            {
                pass = true;
                return Completed(e);
            };
            func(env, next);
            Assert.False(pass);
        }

        [Fact]
        public void ItCallsTheNextFuncWithValidRegex()
        {
            var func = Cors.Create(@"/https:\/\/cors.com/").Build();
            var env = CreateEnv();
            bool pass = false;
            AppFunc next = e =>
            {
                pass = true;
                return Completed(e);
            };
            func(env, next);
            Assert.True(pass);
        }

        [Fact]
        public void ItDoesNotCallTheNextFuncWithInvalidRegex()
        {
            var func = Cors.Create(@"/https:\/\/reject.com/").Build();
            var env = CreateEnv();
            bool pass = false;
            AppFunc next = e =>
            {
                pass = true;
                return Completed(e);
            };
            func(env, next);
            Assert.False(pass);
        }

        [Fact]
        public void ItCallsTheNextFuncWithValidTestFunc()
        {
            Func<string, bool> endsWithCors = s => s.EndsWith("cors.com", StringComparison.OrdinalIgnoreCase);
            var func = Cors.Create(endsWithCors).Build();
            var env = CreateEnv();
            bool pass = false;
            AppFunc next = e =>
            {
                pass = true;
                return Completed(e);
            };
            func(env, next);
            Assert.True(pass);
        }

        [Fact]
        public void ItDoesNotCallTheNextFuncWithInvalidTestFunc()
        {
            Func<string, bool> endsWithReject = s => s.EndsWith("reject.com", StringComparison.OrdinalIgnoreCase);
            var func = Cors.Create(endsWithReject).Build();
            var env = CreateEnv();
            bool pass = false;
            AppFunc next = e =>
            {
                pass = true;
                return Completed(e);
            };
            func(env, next);
            Assert.False(pass);
        }

        [Fact]
        public void ItSetsTheDefaultResponseStatusWhenRejecting()
        {
            var func = Cors.Create(new OriginSetMatcher("https://reject.com")).Build();
            var env = CreateEnv();
            func(env, Completed);
            Assert.Equal(403, env[OwinKeys.StatusCode]);
        }
        
        [Fact]
        public void ItSetsTheSpecifiedResponseStatusWhenRejecting()
        {
            var func = Cors.Create(new OriginSetMatcher("https://reject.com")).UseStopStatus(500).Build();
            var env = CreateEnv();
            func(env, Completed);
            Assert.Equal(500, env[OwinKeys.StatusCode]);
        }

        private static Dictionary<string, object> CreateEnv()
        {
            return new Dictionary<string, object>
            {
                {
                    OwinKeys.RequestHeaders, new Dictionary<string, string[]>
                    {
                        {"Host", new[] {"https://cors.com"}}
                    }
                }
            };
        }

        private static Task Completed(IDictionary<string,object> _)
        {
            var tcs = new TaskCompletionSource<int>();
            tcs.SetResult(0);
            return tcs.Task;
        }
    }
}
