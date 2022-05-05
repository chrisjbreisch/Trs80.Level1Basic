using Trs80.Level1Basic.Interpreter.Scanner;

namespace Trs80.Level1Basic.CommandModels;

public interface IScanModel
{
    string SourceLine { get; set; }
    List<Token> Tokens { get; set; }
}