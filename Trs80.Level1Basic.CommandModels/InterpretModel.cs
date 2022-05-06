using Trs80.Level1Basic.Interpreter.Parser;

namespace Trs80.Level1Basic.CommandModels;

public class InterpretModel
{
    public ParsedLine ParsedLine { get; set; } = new();
}