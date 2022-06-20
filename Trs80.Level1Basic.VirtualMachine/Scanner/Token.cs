namespace Trs80.Level1Basic.VirtualMachine.Scanner;

public sealed class Token
{
    public TokenType Type { get; set; }
    public string Lexeme { get; set; }
    public dynamic Literal { get; set; }
    public string SourceLine { get; set; }
    public int LinePosition { get; set; }

    public Token(TokenType type, string lexeme, dynamic literal, string sourceLine, int linePosition)
    {
        Type = type;
        Lexeme = lexeme;
        Literal = literal;
        SourceLine = sourceLine;
        LinePosition = linePosition;
    }

    public override string ToString()
    {
        return $"{Type} {Lexeme} {Literal}";
    }
}