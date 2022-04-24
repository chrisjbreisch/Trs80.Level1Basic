namespace Trs80.Level1Basic.Parser.Expressions
{
    public class SemiColonWithoutNewLinePrintExpression : PrintExpression, IPrintWithoutNewLine
    {
        public SemiColonWithoutNewLinePrintExpression(IExpression left, PrintExpression right) : base(left, right)
        {

        }

        protected override dynamic Evaluate()
        {
            if (Right is NullExpression || string.IsNullOrEmpty(Right.Value))
                return Left.Value;
            return Left.Value + " " + Right.Value;
        }
    }
}