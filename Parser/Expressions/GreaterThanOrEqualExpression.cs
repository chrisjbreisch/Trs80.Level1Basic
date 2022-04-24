namespace Trs80.Level1Basic.Parser.Expressions
{
    public class GreaterThanOrEqualExpression : Expression
    {
        public GreaterThanOrEqualExpression(IExpression left, IExpression right) : base(left, right)
        {
        }

        protected override dynamic Evaluate()
        {
            return Left.Value >= Right.Value;
        }
    }
}