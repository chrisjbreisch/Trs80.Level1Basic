using System;
using System.Collections.Generic;

namespace Trs80.Level1Basic.Services.Interpreter;

public class FunctionDefinition
{
    public int Arity { get; set; }
    public Func<IBasicInterpreter, List<dynamic>, dynamic> Call;
}