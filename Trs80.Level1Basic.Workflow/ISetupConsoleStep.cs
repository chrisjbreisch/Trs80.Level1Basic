using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Trs80.Level1Basic.Workflow;

public interface ISetupConsoleStep : IStepBody
{
    ExecutionResult Run(IStepExecutionContext context);
}