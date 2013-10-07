namespace Simple.Owin.Cors
{
    using System.Text.RegularExpressions;

    public class OriginRegexMatcher : IOriginMatcher
    {
        private readonly Regex _regex;

        public OriginRegexMatcher(string pattern)
        {
            _regex = new Regex(pattern, RegexOptions.Compiled);
        }

        public bool IsMatch(string origin)
        {
            return _regex.IsMatch(origin);
        }
    }
}