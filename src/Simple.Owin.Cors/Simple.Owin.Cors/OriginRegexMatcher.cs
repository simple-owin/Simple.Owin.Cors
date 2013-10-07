namespace Simple.Owin.Cors
{
    using System.Text.RegularExpressions;

    public class OriginRegexMatcher : OriginMatcher
    {
        private readonly Regex _pattern;

        public OriginRegexMatcher(string pattern)
        {
            _pattern = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public OriginRegexMatcher(Regex pattern)
        {
            _pattern = pattern;
        }

        public override bool IsMatch(string origin)
        {
            return _pattern.IsMatch(origin);
        }
    }
}