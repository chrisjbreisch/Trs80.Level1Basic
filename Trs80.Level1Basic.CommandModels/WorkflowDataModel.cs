using Trs80.Level1Basic.Interpreter.Parser;
using Trs80.Level1Basic.Interpreter.Scanner;

namespace Trs80.Level1Basic.CommandModels;

public sealed class WorkflowDataModel : IWorkflowDataModel
{
    public bool WritePrompt { get; set; } = false;
    public bool Done { get; set; }
    public string SourceLine { get; set; } = string.Empty;
    public List<Token> Tokens { get; set; } = new();
    public ParsedLine ParsedLine { get; set; } = new();

    public WorkflowDataModel()
    {

    }
}