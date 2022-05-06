using Trs80.Level1Basic.Interpreter.Parser;
using Trs80.Level1Basic.Interpreter.Scanner;

namespace Trs80.Level1Basic.CommandModels;

public class ParseModel
{
    public List<Token> Tokens { get; set; } = new();
    public ParsedLine ParsedLine { get; set; } = new();
}