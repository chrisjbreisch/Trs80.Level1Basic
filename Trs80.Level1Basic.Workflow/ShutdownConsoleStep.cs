using System.Diagnostics.CodeAnalysis;

using Trs80.Level1Basic.Command;
using Trs80.Level1Basic.CommandModels;

using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Trs80.Level1Basic.Workflow;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class ShutdownConsoleStep : StepBody, IShutdownConsoleStep
{
    private readonly ICommand<ShutdownConsoleModel> _command;

    public ShutdownConsoleStep(ICommand<ShutdownConsoleModel> inboundCommand)
    {
        _command = inboundCommand ?? throw new ArgumentNullException(nameof(inboundCommand));
    }

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        _command.Execute(new ShutdownConsoleModel());

        return ExecutionResult.Next();
    }
}