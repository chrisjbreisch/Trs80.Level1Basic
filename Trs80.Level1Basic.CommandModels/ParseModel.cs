using Trs80.Level1Basic.VirtualMachine.Parser;
using Trs80.Level1Basic.VirtualMachine.Scanner;

namespace Trs80.Level1Basic.CommandModels;

public class ParseModel
{
    public List<Token> Tokens { get; set; } = new();
    public ParsedLine ParsedLine { get; set; } = new();
}