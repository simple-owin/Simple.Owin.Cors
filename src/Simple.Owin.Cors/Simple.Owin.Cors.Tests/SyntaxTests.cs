namespace Simple.Owin.CorsMiddleware.Tests
{
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
    }
}