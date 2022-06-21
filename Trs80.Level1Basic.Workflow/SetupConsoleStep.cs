using Trs80.Level1Basic.Command;
using Trs80.Level1Basic.CommandModels;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Trs80.Level1Basic.Workflow;

public class SetupConsoleStep : StepBody, ISetupConsoleStep
{
    private readonly ICommand<SetupTrs80Model> _command;

    public SetupConsoleStep(ICommand<SetupTrs80Model> command)
    {
        _command = command ?? throw new ArgumentNullException(nameof(command));
    }

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        _command.Execute(new SetupTrs80Model());

        return ExecutionResult.Next();
    }
}