﻿using Trs80.Level1Basic.Command;
using Trs80.Level1Basic.CommandModels;
using Trs80.Level1Basic.Interpreter.Parser;
using Trs80.Level1Basic.Interpreter.Scanner;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Trs80.Level1Basic.Workflow;

public class ParseStep : StepBody, IParseStep
{
    private readonly ICommand<ParseModel> _command;
    public List<Token> Tokens { get; set;} = new ();
    public ParsedLine ParsedLine { get; set; } = new();

    public ParseStep(ICommand<ParseModel> inboundCommand)
    {
        _command = inboundCommand ?? throw new ArgumentNullException(nameof(inboundCommand));
    }

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        var model = new ParseModel {
            Tokens = Tokens
        };

        _command.Execute(model);

        ParsedLine = model.ParsedLine;

        return ExecutionResult.Next();
    }
}