namespace Simple.Owin.CorsMiddleware
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;

    internal class Builder
    {
        private readonly ParameterExpression _env = Expression.Parameter(typeof (IDictionary<string, object>));
        private readonly ParameterExpression _next = Expression.Parameter(typeof (Func<IDictionary<string,object>, Task>));
        private readonly ConstantExpression _hostKey = Expression.Constant("Host");

        private readonly OriginMatcher[] _matchers;

        public Builder(IEnumerable<OriginMatcher> matchers)
        {
            _matchers = matchers.ToArray();
        }

        public bool? AllowCredentials { get; set; }

        public string[] AllowMethods { get; set; }
        public string[] AllowHeaders { get; set; }
        public string[] ExposeHeaders { get; set; }

        public double? MaxAge { get; set; }

        public int StopStatus { get; set; }

        public Func<IDictionary<string, object>, Func<IDictionary<string, object>, Task>, Task> Build()
        {
            var isAllowed = BuildCheckBlock();

            var block = BuildFinalBlock(isAllowed);

            var lambda = Expression.Lambda<Func<IDictionary<string, object>, Func<IDictionary<string, object>, Task>, Task>>(block, _env, _next);

            return lambda.Compile();
        }

        private BlockExpression BuildCheckBlock()
        {
            if (_matchers.Length == 1 && _matchers[0] is OriginMatcher.WildcardMatcher)
            {
                return BuildWildcardBlock();
            }
            return BuildOriginMatchingBlock();
        }

        private BlockExpression BuildFinalBlock(Expression isAllowed)
        {
            var task = Expression.Variable(typeof (Task));
            var stopStatus = Expression.Constant(StopStatus);

            var setHeadersBlock = BuildSetHeadersBlock();

            Expression allowedBlock = Expression.Assign(task, Expression.Invoke(_next, _env));

            if (setHeadersBlock.Count > 0)
            {
                setHeadersBlock.Add(allowedBlock);
                allowedBlock = Expression.Block(new[] {task}, setHeadersBlock);
            }

            var ifThen =
                (Expression.IfThenElse(isAllowed, allowedBlock,
                    Expression.Assign(task, Expression.Call(Methods.Stop, _env, stopStatus))));

            var block = Expression.Block(new[] {task}, ifThen, task);
            return block;
        }

        private IList<Expression> BuildSetHeadersBlock()
        {
            var block = new List<Expression>();
            if (AllowCredentials.GetValueOrDefault())
            {
                block.Add(BuildSetHeaderCall(HeaderKeys.AccessControlAllowCredentials, "true"));
            }

            if (AllowMethods != null && AllowMethods.Length > 0)
            {
                if (AllowMethods.Length == 1 && AllowMethods[0] == "*")
                {
                    block.Add(Expression.Call(Methods.MirrorRequestMethods, _env));
                }
                else
                {
                    block.Add(BuildSetHeaderCall(HeaderKeys.AccessControlAllowMethods, AllowMethods));
                }
            }

            if (AllowHeaders != null && AllowHeaders.Length > 0)
            {
                if (AllowHeaders.Length == 1 && AllowHeaders[0] == "*")
                {
                    block.Add(Expression.Call(Methods.MirrorRequestHeaders, _env));
                }
                else
                {
                    block.Add(BuildSetHeaderCall(HeaderKeys.AccessControlAllowHeaders, AllowHeaders));
                }
            }

            if (ExposeHeaders != null && ExposeHeaders.Length > 0)
            {
                block.Add(BuildSetHeaderCall(HeaderKeys.AccessControlExposeHeaders, ExposeHeaders));
            }

            if (MaxAge.HasValue)
            {
                block.Add(BuildSetHeaderCall(HeaderKeys.AccessControlMaxAge, MaxAge.Value.ToString(CultureInfo.InvariantCulture)));
            }

            return block;
        }

        private Expression BuildSetHeaderCall(string key, string[] values)
        {
            return BuildSetHeaderCall(key, string.Join(", ", values));
        }

        private Expression BuildSetHeaderCall(string key, string value)
        {
            var keyConstant = Expression.Constant(key);
            var valueConstant = Expression.Constant(value);
            return Expression.Call(Methods.SetResponseHeaderValue, _env, keyConstant, valueConstant);
        }

        private BlockExpression BuildOriginMatchingBlock()
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
}