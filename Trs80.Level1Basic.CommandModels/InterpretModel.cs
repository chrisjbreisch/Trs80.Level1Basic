using Trs80.Level1Basic.Interpreter.Parser;
using Trs80.Level1Basic.Interpreter.Scanner;

namespace Trs80.Level1Basic.CommandModels;

public class InterpretModel
{
    public ParsedLine ParsedLine { get; set; } = new();
}