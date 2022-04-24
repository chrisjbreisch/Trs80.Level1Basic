namespace Trs80.Level1Basic.Parser.Expressions
{
    public class NotEqualExpression : Expression
    {
        public NotEqualExpression(IExpression left, IExpression right) : base(left, right)
        {
        }

        protected override dynamic Evaluate()
        {
            return Left.Value != Right.Value;
        }
    }
}