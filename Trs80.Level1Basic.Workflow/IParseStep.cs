using System.Diagnostics.CodeAnalysis;

using Trs80.Level1Basic.VirtualMachine.Parser.Statements;
using Trs80.Level1Basic.VirtualMachine.Scanner;

using WorkflowCore.Interface;

namespace Trs80.Level1Basic.Workflow;

public interface IParseStep : IStepBody
{
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
    List<Token> Tokens { get; set; }
    IStatement Statement { get; set; }
}