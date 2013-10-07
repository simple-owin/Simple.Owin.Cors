namespace Simple.Owin.Cors
{
    public interface IOriginMatcher
    {
        bool IsMatch(string origin);
    }
}