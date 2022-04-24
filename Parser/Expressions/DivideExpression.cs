namespace Trs80.Level1Basic.Parser.Expressions
{
    public class DivideExpression : Expression
    {
        public DivideExpression(IExpression left, IExpression right) : base(left, right)
        {
        }

        protected override dynamic Evaluate()
        {
            return (float)Left.Value / Right.Value;
            ////if (!IsCalculated)
            //    Value = Left.Value / Right.Value;

            //return Value;
        }
    }
}