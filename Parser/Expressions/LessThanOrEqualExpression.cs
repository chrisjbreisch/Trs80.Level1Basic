namespace Trs80.Level1Basic.Parser.Expressions
{
    public class LessThanOrEqualExpression : Expression
    {
        public LessThanOrEqualExpression(IExpression left, IExpression right) : base(left, right)
        {
        }

        protected override dynamic Evaluate()
        {
            return Left.Value <= Right.Value;
        }

    }
}