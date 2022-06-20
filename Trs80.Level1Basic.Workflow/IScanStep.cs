using Trs80.Level1Basic.Common;
using Trs80.Level1Basic.VirtualMachine.Scanner;
using WorkflowCore.Interface;

namespace Trs80.Level1Basic.Workflow;

public interface IScanStep : IStepBody
{
    SourceLine SourceLine { get; set; }
    List<Token> Tokens { get; set; }
}