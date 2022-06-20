using System.Diagnostics.CodeAnalysis;
using Trs80.Level1Basic.Common;
using Trs80.Level1Basic.VirtualMachine.Parser.Statements;
using Trs80.Level1Basic.VirtualMachine.Scanner;

namespace Trs80.Level1Basic.CommandModels;

[SuppressMessage("ReSharper", "EmptyConstructor")]
public sealed class WorkflowDataModel
{
    public bool WritePrompt { get; set; } = false;
    public bool Done { get; set; }
    public SourceLine SourceLine { get; set; } = new();
    public List<Token> Tokens { get; set; } = new();
    public Statement Statement { get; set; } = null!;

    public WorkflowDataModel()
    {

    }
}