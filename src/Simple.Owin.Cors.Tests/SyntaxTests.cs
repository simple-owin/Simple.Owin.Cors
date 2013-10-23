namespace Simple.Owin.CorsMiddleware.Tests
{
    using System.Text.RegularExpressions;
    using Xunit;

    public class SyntaxTests
    {
        [Fact]
        public void ImplicitCastFromStringWorks()
        {
            Assert.DoesNotThrow(() => Cors.Create("http://cors.com"));
        }

        [Fact]
        public void ImplicitCastsFromStringsWorks()
        {
            Assert.DoesNotThrow(() => Cors.Create("http://cors.com", "http://owin.org"));
        }

        [Fact]
        public void ImplicitCastFromRegexWorks()
        {
            Assert.DoesNotThrow(() => Cors.Create(new Regex(@".*cors\.com")));
        }
    }
}