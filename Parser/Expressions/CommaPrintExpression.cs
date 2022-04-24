using System;

namespace Trs80.Level1Basic.Parser.Expressions
{
    public class CommaPrintExpression : PrintExpression, IPrintNewLine
    {
        public CommaPrintExpression(IExpression left, PrintExpression right) : base(left, right)
        {

        }

        protected override dynamic Evaluate()
        {
            string left = Left.ToString();
            string right = Right.ToString();
            int leftPad = GetPadAmount(left);
            int rightPad = GetPadAmount(right);
            var x = Left;
            return Left.ToString().PadRight(leftPad) + Right.ToString().PadRight(rightPad);
        }

        private int GetPadAmount(string left)
        {
            int zones = (int)Math.Ceiling(left.Length / 15.0);
            return zones * 15;
        }
    }
}