using System.Diagnostics.CodeAnalysis;

using Trs80.Level1Basic.Command;
using Trs80.Level1Basic.CommandModels;
using Trs80.Level1Basic.Common;

using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Trs80.Level1Basic.Workflow;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class InputStep : StepBody, IInputStep
{
    private readonly ICommand<InputModel> _command;
    public bool WritePrompt { get; set; }
    public bool Done { get; set; }
    public SourceLine SourceLine { get; set; } = new();

    public InputStep(ICommand<InputModel> inboundCommand)
    {
        _command = inboundCommand ?? throw new ArgumentNullException(nameof(inboundCommand));
    }

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        var model = new InputModel
        {
            WritePrompt = WritePrompt
        };

        _command.Execute(model);

        Done = model.Done;
        SourceLine = model.SourceLine;

        return ExecutionResult.Next();
    }
}