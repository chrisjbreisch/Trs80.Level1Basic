using System.Diagnostics.CodeAnalysis;

using Trs80.Level1Basic.Command;
using Trs80.Level1Basic.CommandModels;

using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Trs80.Level1Basic.Workflow;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class SetupConsoleStep : StepBody, ISetupConsoleStep
{
    private readonly ICommand<SetupConsoleModel> _command;

    public SetupConsoleStep(ICommand<SetupConsoleModel> command)
    {
        _command = command ?? throw new ArgumentNullException(nameof(command));
    }

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        _command.Execute(new SetupConsoleModel());

        return ExecutionResult.Next();
    }
}