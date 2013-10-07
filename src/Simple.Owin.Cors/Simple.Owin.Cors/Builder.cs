namespace Simple.Owin.Cors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;

    internal class Builder
    {
        private readonly ParameterExpression _env = Expression.Parameter(typeof (IDictionary<string, object>));
        private readonly ParameterExpression _next = Expression.Parameter(typeof (Func<Task>));
        private readonly ConstantExpression _hostKey = Expression.Constant("Host");
        private readonly List<Expression> _blocks = new List<Expression>();

        private readonly IOriginMatcher[] _matchers;

        public Builder(IEnumerable<IOriginMatcher> matchers)
        {
            _matchers = matchers.ToArray();
        }

        public Func<IDictionary<string, object>, Func<Task>, Task> Build()
        {
            BlockExpression isAllowed;
            if (_matchers.Length == 1 && _matchers[0] is OriginMatchers.WildcardMatcher)
            {
                isAllowed = BuildWildcardBlock();
            }
            else
            {
                isAllowed = BuildOriginCheckBlock();
            }

            var invoke = Expression.Invoke(_next);

            var call = Expression.Call(Methods.Completed);

            var task = Expression.Variable(typeof (Task));

            var ifThen = (Expression.IfThenElse(isAllowed, Expression.Assign(task, invoke), Expression.Assign(task, call)));

            var block = Expression.Block(new[] {task}, ifThen, task);

            var lambda = Expression.Lambda<Func<IDictionary<string, object>, Func<Task>, Task>>(block, _env, _next);

            return lambda.Compile();
        }

        private BlockExpression BuildOriginCheckBlock()
        {
            var blocks = new List<Expression>();
            ParameterExpression allowed = Expression.Variable(typeof (bool));
            var matchTests = new List<Expression>();
            var host = Expression.Variable(typeof (string));
            var assignHost = Expression.Assign(host, Expression.Call(Methods.GetRequestHeaderValue, _env, _hostKey));
            blocks.Add(assignHost);
            var hostIsSet = Expression.Not(Expression.Call(Methods.StringIsNullOrWhitespace, host));

            foreach (var expressionMatcher in _matchers)
            {
                var matcher = Expression.Constant(expressionMatcher);
                var isMatchMethod = expressionMatcher.GetType().GetMethod("IsMatch", new[] {typeof (string)});
                matchTests.Add(Expression.Call(matcher, isMatchMethod, host));
            }

            var isMatch = matchTests.Aggregate(Expression.Or);

            blocks.Add(Expression.Assign(allowed, Expression.And(hostIsSet, isMatch)));
            blocks.Add(Expression.IfThen(allowed, Expression.Call(Methods.SetResponseHeaderValue, _env,
                Expression.Constant(HeaderKeys.AccessControlAllowOrigin), host)));
            blocks.Add(allowed);
            return Expression.Block(new[] {host, allowed}, blocks);
        }

        private BlockExpression BuildWildcardBlock()
        {
            var blocks = new List<Expression>
            {
                Expression.Call(Methods.SetResponseHeaderValue, _env,
                    Expression.Constant(HeaderKeys.AccessControlAllowOrigin), Expression.Constant("*")),
                Expression.Constant(true)
            };
            return Expression.Block(blocks);
        }
    }

    public static class HeaderKeys
    {
        /// <summary>
        /// The <c>Access-Control-Allow-Origin</c> response header field.
        /// </summary>
        public const string AccessControlAllowOrigin = "Access-Control-Allow-Origin";

        /// <summary>
        /// The <c>Access-Control-Allow-Methods</c> response header field.
        /// </summary>
        public const string AccessControlAllowMethods = "Access-Control-Allow-Methods";

        /// <summary>
        /// The <c>Access-Control-Allow-Headers</c> response header field.
        /// </summary>
        public const string AccessControlAllowHeaders = "Access-Control-Allow-Methods";

        /// <summary>
        /// The <c>Access-Control-Expose-Headers</c> response header field.
        /// </summary>
        public const string AccessControlExposeHeaders = "Access-Control-Allow-Methods";

        /// <summary>
        /// The <c>Access-Control-Max-Age</c> response header field.
        /// </summary>
        public const string AccessControlMaxAge = "Access-Control-Allow-Methods";

        /// <summary>
        /// The <c>Access-Control-Allow-Credentials</c> response header field.
        /// </summary>
        public const string AccessControlAllowCredentials = "Access-Control-Allow-Methods";
    }
}