namespace Trs80.Level1Basic.Parser.Expressions
{
    public class LiteralExpression<T> : ILeafExpression
    {
        public LiteralExpression()
        {

        }
        public LiteralExpression(T value)
        {
            Left = null;
            Right = null;
            Value = value;
        }
        public override string ToString()
        {
            return Value.ToString();
        }

        public IExpression Left { get; set; }
        public IExpression Right { get; set; }
        public object Value { get; set; }

        public void Initialize()
        {
            // do nothing
        }
    }
}