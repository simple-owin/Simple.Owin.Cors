namespace Simple.Owin.Cors.Tests
{
    using Xunit;

    public class SyntaxTests
    {
        [Fact]
        public void ImplicitCastFromStringWorks()
        {
            Assert.DoesNotThrow(() => CorsMiddleware.Create("http://cors.com"));
        }

        [Fact]
        public void ImplicitCastsFromStringsWorks()
        {
            Assert.DoesNotThrow(() => CorsMiddleware.Create("http://cors.com", "http://owin.org"));
        }
    }
}