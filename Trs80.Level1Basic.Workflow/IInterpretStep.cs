using Trs80.Level1Basic.VirtualMachine.Parser.Statements;
using WorkflowCore.Interface;

namespace Trs80.Level1Basic.Workflow;

public interface IInterpretStep : IStepBody
{
    Statement Statement { get; set; }
}