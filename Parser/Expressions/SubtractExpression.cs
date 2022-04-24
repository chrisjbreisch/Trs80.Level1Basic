namespace Trs80.Level1Basic.Parser.Expressions
{
    public class SubtractExpression : Expression
    {
        public SubtractExpression(IExpression left, IExpression right) : base(left, right)
        {
        }

        protected override dynamic Evaluate()
        {
            return Left.Value - Right.Value;
            //// (!IsCalculated)
            //Value = Left.Value - Right.Value;

            //return Value;
        }
    }
}