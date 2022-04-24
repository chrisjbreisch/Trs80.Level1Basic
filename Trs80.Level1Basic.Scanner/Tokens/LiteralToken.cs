namespace Trs80.Level1Basic.Scanner.Tokens
{
    public class LiteralToken<T> : IToken
    {
        public LiteralToken(T value)
        {
            Value = value;
        }

        public T Value { get; set; }
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}