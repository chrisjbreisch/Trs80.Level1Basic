namespace Trs80.Level1Basic.Parser.Expressions
{
    public class SemiColonPrintExpression : PrintExpression, IPrintNewLine
    {
        public SemiColonPrintExpression(IExpression left, PrintExpression right) : base(left, right)
        {

        }

        protected override dynamic Evaluate()
        {
            return Left.Value + " " + Right.Value;
        }
    }
}