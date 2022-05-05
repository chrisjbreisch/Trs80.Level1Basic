using Trs80.Level1Basic.Interpreter.Parser;
using Trs80.Level1Basic.Interpreter.Scanner;

using WorkflowCore.Interface;

namespace Trs80.Level1Basic.Workflow;

public interface IParseStep : IStepBody
{
    List<Token> Tokens { get; set; }
    ParsedLine ParsedLine { get; set; }
}