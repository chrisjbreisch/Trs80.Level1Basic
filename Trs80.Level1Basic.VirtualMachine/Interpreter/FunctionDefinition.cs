using System;
using System.Collections.Generic;

namespace Trs80.Level1Basic.VirtualMachine.Interpreter;

public class FunctionDefinition
{
    public int Arity { get; set; }
    public Func<IInterpreter, List<dynamic>, dynamic> Call;
}