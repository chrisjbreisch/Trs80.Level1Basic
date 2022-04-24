namespace Trs80.Level1Basic.Parser.Expressions
{
    public class MultiplyExpression : Expression
    {
        public MultiplyExpression(IExpression left, IExpression right) : base(left, right)
        {
        }

        protected override dynamic Evaluate()
        {
            //// (!IsCalculated)
            //    Value = Left.Value * Right.Value;

            //return Value;
            return Left.Value * Right.Value;
        }
    }
}