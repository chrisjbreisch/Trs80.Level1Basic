﻿using System;
using System.Collections.Generic;

namespace Trs80.Level1Basic.VirtualMachine.Machine;

public interface ICallable
{
    int Arity { get; set; }
    Func<ITrs80, List<dynamic>, dynamic> Call { get; set; }
}

public class Callable : ICallable
{
    public int Arity { get; set; }
    public Func<ITrs80, List<dynamic>, dynamic> Call { get; set; }
}