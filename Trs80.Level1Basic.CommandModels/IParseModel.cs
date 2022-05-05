using Trs80.Level1Basic.Interpreter.Parser;
using Trs80.Level1Basic.Interpreter.Scanner;

namespace Trs80.Level1Basic.CommandModels;

public interface IParseModel
{
    List<Token> Tokens { get; set; }
    ParsedLine ParsedLine { get; set; }
}