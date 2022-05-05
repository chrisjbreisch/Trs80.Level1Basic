using Trs80.Level1Basic.Interpreter.Parser;
using Trs80.Level1Basic.Interpreter.Scanner;

namespace Trs80.Level1Basic.CommandModels;

public interface IWorkflowDataModel
{
    bool WritePrompt { get; set; }
    bool Done { get; set; }
    string SourceLine { get; set; }
    List<Token> Tokens { get; set; }
    ParsedLine ParsedLine { get; set; }
}