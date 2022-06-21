using System;
using System.Collections.Generic;

namespace Trs80.Level1Basic.VirtualMachine.Machine;

public interface ICallable
{
    int Arity { get; set; }
    Func<ITrs80Api, List<dynamic>, dynamic> Call { get; set; }
}