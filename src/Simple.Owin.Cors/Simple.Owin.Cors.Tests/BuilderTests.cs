using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Owin.Cors.Tests
{
    using Xunit;

    public class BuilderTests
    {
        [Fact]
        public void ItBuildsACallableFunc()
        {
            var func = CorsMiddleware.Build(OriginMatchers.Wildcard);
            var env = new Dictionary<string, object>();
            Func<Task> next = Completed;
            Assert.DoesNotThrow(() => func(env, next));
        }

        [Fact]
        public void ItCallsTheNextFuncWithWildcard()
        {
            var func = CorsMiddleware.Build(OriginMatchers.Wildcard);
            var env = new Dictionary<string, object>();
            bool pass = false;
            Func<Task> next = () =>
            {
                pass = true;
                return Completed();
            };
            func(env, next);
            Assert.True(pass);
        }

        [Fact]
        public void ItCallsTheNextFuncWithSet()
        {
            var func = CorsMiddleware.Build(new[] {new OriginSetMatcher("https://cors.com")});
            var env = new Dictionary<string, object>
            {
                {
                    OwinKeys.RequestHeaders, new Dictionary<string, string[]>
                    {
                        {"Host", new[] {"https://cors.com"}}
                    }
                }
            };
            bool pass = false;
            Func<Task> next = () =>
            {
                pass = true;
                return Completed();
            };
            func(env, next);
            Assert.True(pass);
        }

        private static Task Completed()
        {
            var tcs = new TaskCompletionSource<int>();
            tcs.SetResult(0);
            return tcs.Task;
        }
    }
}
