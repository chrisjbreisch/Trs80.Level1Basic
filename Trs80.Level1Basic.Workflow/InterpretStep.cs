using Trs80.Level1Basic.Command;
using Trs80.Level1Basic.CommandModels;
using Trs80.Level1Basic.VirtualMachine.Parser.Statements;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Trs80.Level1Basic.Workflow;

public class InterpretStep : StepBody, IInterpretStep
{
    private readonly ICommand<InterpretModel> _command;
    public Statement Statement { get; set; } = null!;

    public InterpretStep(ICommand<InterpretModel> inboundCommand)
    {
        _command = inboundCommand ?? throw new ArgumentNullException(nameof(inboundCommand));
    }

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        var model = new InterpretModel
        {
            Statement = Statement
        };

        _command.Execute(model);

        return ExecutionResult.Next();
    }
}