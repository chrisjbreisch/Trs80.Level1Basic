using Trs80.Level1Basic.Interpreter.Scanner;
using WorkflowCore.Interface;

namespace Trs80.Level1Basic.Workflow;

public interface IScanStep : IStepBody
{
    string SourceLine { get; set; }
    List<Token> Tokens { get; set; }
}