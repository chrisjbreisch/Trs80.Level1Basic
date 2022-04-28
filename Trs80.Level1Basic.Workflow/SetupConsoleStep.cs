using Trs80.Level1Basic.Command;
using Trs80.Level1Basic.CommandModels;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Trs80.Level1Basic.Workflow;

public class SetupConsoleStep : StepBody, ISetupConsoleStep
{
    private readonly ICommand<SetupConsoleModel> _command;

    public SetupConsoleStep(ICommand<SetupConsoleModel> inboundCommand)
    {
        _command = inboundCommand ?? throw new ArgumentNullException(nameof(inboundCommand));
    }

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        _command.Execute(new SetupConsoleModel());

        return ExecutionResult.Next();
    }
}