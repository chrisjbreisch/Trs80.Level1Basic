using Trs80.Level1Basic.Command;
using Trs80.Level1Basic.CommandModels;

using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Trs80.Level1Basic.Workflow;

public class InputStep : StepBody, IInputStep
{
    private readonly ICommand<InputModel> _command;
    public bool WritePrompt { get; set; }
    public bool Done { get; set; }
    public string SourceLine { get; set; } = string.Empty;

    public InputStep(ICommand<InputModel> inboundCommand)
    {
        _command = inboundCommand ?? throw new ArgumentNullException(nameof(inboundCommand));
    }

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        var model = new InputModel {
            WritePrompt = WritePrompt
        };

        _command.Execute(model);

        Done = model.Done;
        SourceLine = model.SourceLine;

        return ExecutionResult.Next();
    }
}