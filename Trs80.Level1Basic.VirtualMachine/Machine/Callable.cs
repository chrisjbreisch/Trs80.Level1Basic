using System;
using System.Collections.Generic;

namespace Trs80.Level1Basic.VirtualMachine.Machine;

public class Callable : ICallable
{
    public int Arity { get; set; }
    public Func<ITrs80, List<dynamic>, dynamic> Call { get; set; }
}