using Trs80.Level1Basic.Command;
using Trs80.Level1Basic.CommandModels;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Trs80.Level1Basic.Workflow
{
    public class InterpreterStep : StepBody, IInterpreterStep
    {
        private readonly ICommand<InterpreterModel> _command;

        public InterpreterStep(ICommand<InterpreterModel> inboundCommand)
        {
            _command = inboundCommand ?? throw new ArgumentNullException(nameof(inboundCommand));
        }

        public override ExecutionResult Run(IStepExecutionContext context)
        {
            _command.Execute(new InterpreterModel());

            return ExecutionResult.Next();
        }
    }
}