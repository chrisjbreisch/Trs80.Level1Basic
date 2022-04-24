namespace Trs80.Level1Basic.Parser.Expressions
{
    public class PrintExpression : Expression
    {
        public PrintExpression(IExpression left, PrintExpression right) : base(left, right)
        {

        }

        protected override dynamic Evaluate()
        {
            return Left + Right.ToString();
        }
    }
}