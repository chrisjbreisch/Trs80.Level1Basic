namespace Trs80.Level1Basic.Parser.Expressions
{
    public class EqualExpression : Expression
    {
        public EqualExpression(IExpression left, IExpression right) : base(left, right)
        {
        }

        protected override dynamic Evaluate()
        {
            return Left.Value == Right.Value;
        }
    }
}