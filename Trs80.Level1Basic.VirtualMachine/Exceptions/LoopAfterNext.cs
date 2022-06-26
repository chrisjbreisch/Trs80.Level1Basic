using System;
using Trs80.Level1Basic.VirtualMachine.Parser.Statements;

namespace Trs80.Level1Basic.VirtualMachine.Exceptions;

public class LoopAfterNext : Exception
{
    public Next Next { get; }

    public LoopAfterNext(Next next)
    {
        Next = next;
    }
}