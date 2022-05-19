using Trs80.Level1Basic.Command;
using Trs80.Level1Basic.CommandModels;
using Trs80.Level1Basic.VirtualMachine.Parser;
using Trs80.Level1Basic.VirtualMachine.Parser.Statements;
using Trs80.Level1Basic.VirtualMachine.Scanner;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Trs80.Level1Basic.Workflow;

public interface IParseStep : IStepBody
{
    List<Token> Tokens { get; set; }
    Statement Statement { get; set; }
}

public class ParseStep : StepBody, IParseStep
{
    private readonly ICommand<ParseModel> _command;
    public List<Token> Tokens { get; set; } = new();
    public Statement Statement { get; set; } = null!;

    public ParseStep(ICommand<ParseModel> inboundCommand)
    {
        _command = inboundCommand ?? throw new ArgumentNullException(nameof(inboundCommand));
    }

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        var model = new ParseModel
        {
            Tokens = Tokens
        };

        _command.Execute(model);

        Statement = model.Statement;

        return ExecutionResult.Next();
    }
}