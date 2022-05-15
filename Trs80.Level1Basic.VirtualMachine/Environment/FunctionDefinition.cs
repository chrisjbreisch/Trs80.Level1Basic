using System;
using System.Collections.Generic;

namespace Trs80.Level1Basic.VirtualMachine.Environment;

public class FunctionDefinition
{
    public int Arity { get; set; }
    public Func<IMachine, List<dynamic>, dynamic> Call;
}