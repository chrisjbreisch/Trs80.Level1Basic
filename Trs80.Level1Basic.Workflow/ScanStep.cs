using Trs80.Level1Basic.Command;
using Trs80.Level1Basic.CommandModels;
using Trs80.Level1Basic.Interpreter.Scanner;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Trs80.Level1Basic.Workflow;

public class ScanStep : StepBody, IScanStep
{
    private readonly ICommand<ScanModel> _command;
    public string SourceLine { get; set; } = string.Empty;
    public List<Token> Tokens { get; set;} = new ();

    public ScanStep(ICommand<ScanModel> inboundCommand)
    {
        _command = inboundCommand ?? throw new ArgumentNullException(nameof(inboundCommand));
    }

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        var model = new ScanModel {
            SourceLine = SourceLine
        };

        _command.Execute(model);

        Tokens = model.Tokens;

        return ExecutionResult.Next();
    }
}