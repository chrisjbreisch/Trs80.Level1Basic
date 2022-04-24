namespace Trs80.Level1Basic.Scanner.Tokens
{
    public class LetterToken : IToken
    {
        public string Value { get; set; }

        public LetterToken(string value)
        {
            Value = value;
        }
    }
}