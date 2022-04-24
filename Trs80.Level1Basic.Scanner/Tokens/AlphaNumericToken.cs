namespace Trs80.Level1Basic.Scanner.Tokens
{
    public class AlphaNumericToken : IToken
    {
        public string Value { get; set; }

        public AlphaNumericToken(string value)
        {
            Value = value;
        }
    }
}