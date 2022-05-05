using Trs80.Level1Basic.Command;
using Trs80.Level1Basic.CommandModels;
using Trs80.Level1Basic.Interpreter.Parser;

using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Trs80.Level1Basic.Workflow;

public interface IInterpretStep : IStepBody
{
    ParsedLine ParsedLine { get; set; }
}

public class InterpretStep : StepBody, IInterpretStep
{
    private readonly ICommand<InterpretModel> _command;
    public ParsedLine ParsedLine { get; set; } = new();

    public InterpretStep(ICommand<InterpretModel> inboundCommand)
    {
        _command = inboundCommand ?? throw new ArgumentNullException(nameof(inboundCommand));
    }

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        var model = new InterpretModel
        {
            ParsedLine = ParsedLine
        };

        _command.Execute(model);

        return ExecutionResult.Next();
    }
}