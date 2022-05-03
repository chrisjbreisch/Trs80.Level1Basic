using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Trs80.Level1Basic.Command;
using Trs80.Level1Basic.CommandModels;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Trs80.Level1Basic.Workflow;

public class SetupConsoleStep : StepBody, ISetupConsoleStep
{
    private readonly ICommand<SetupConsoleModel> _command;
    private readonly ILogger<SetupConsoleStep> _logger;

    public SetupConsoleStep(ICommand<SetupConsoleModel> command, ILoggerFactory logFactory)
    {
        _command = command ?? throw new ArgumentNullException(nameof(command));
        if (logFactory == null) throw new ArgumentNullException(nameof(logFactory));
        _logger = logFactory.CreateLogger<SetupConsoleStep>();
    }

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        Debug.WriteLine("Debug.WriteLine");
        _logger.LogCritical("Boo!");
        _command.Execute(new SetupConsoleModel());

        return ExecutionResult.Next();
    }
}