namespace Trs80.Level1Basic.Interpreter.Scanner;

public sealed class Token
{
    public TokenType Type { get; set; }
    public string Lexeme { get; set; }
    public dynamic Literal { get; set; }
    public string SourceLine { get; set; }

    public Token(TokenType type, string lexeme, dynamic literal, string sourceLine)
    {
        Type = type;
        Lexeme = lexeme;
        Literal = literal;
        SourceLine = sourceLine;
    }

    public override string ToString()
    {
        return $"{Type} {Lexeme} {Literal}";
    }
}