using System.Collections.Generic;

namespace Trs80.Level1Basic.Parser.Expressions
{
    public static class ExpressionList
    {
        private static readonly List<IExpression> _expressions = new List<IExpression>();
        public static void AddExpression(IExpression expression)
        {
            _expressions.Add(expression);
        }

        public static void Initialize()
        {
            foreach (var expression in _expressions)
                expression.Initialize();
        }
    }
}
