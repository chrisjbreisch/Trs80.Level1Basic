using Trs80.Level1Basic.Command;
using Trs80.Level1Basic.CommandModels;

using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Trs80.Level1Basic.Workflow;

public class ShutdownConsoleStep : StepBody, IShutdownConsoleStep
{
    private readonly ICommand<ShutdownTrs80Model> _command;

    public ShutdownConsoleStep(ICommand<ShutdownTrs80Model> inboundCommand)
    {
        _command = inboundCommand ?? throw new ArgumentNullException(nameof(inboundCommand));
    }

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        _command.Execute(new ShutdownTrs80Model());

        return ExecutionResult.Next();
    }
}