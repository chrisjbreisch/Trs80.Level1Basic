namespace Trs80.Level1Basic.Parser.Expressions
{
    public class GreaterThanExpression : Expression
    {
        public GreaterThanExpression(IExpression left, IExpression right) : base(left, right)
        {
        }

        protected override dynamic Evaluate()
        {
            return Left.Value > Right.Value;
        }

    }
}