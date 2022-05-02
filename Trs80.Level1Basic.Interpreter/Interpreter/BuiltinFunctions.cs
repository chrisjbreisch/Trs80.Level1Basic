using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Trs80.Level1Basic.Interpreter.Interpreter;

public interface IBuiltinFunctions
{
    FunctionDefinition Get(string name);
}

[SuppressMessage("ReSharper", "UnusedParameter.Local")]
public class BuiltinFunctions : IBuiltinFunctions
{
    private readonly Dictionary<string, FunctionDefinition> _functions;

    public BuiltinFunctions()
    {
        _functions = new Dictionary<string, FunctionDefinition>
        {
            {"_padquadrant", new FunctionDefinition {Arity = 0, Call = (i,a) => i.Functions.PadQuadrant(i)}},
            {"abs", new FunctionDefinition {Arity = 1, Call = (i, a) => i.Functions.Abs(a[0])}},
            {"a.", new FunctionDefinition {Arity = 1, Call = (i, a) => i.Functions.Abs(a[0])}},
            {"chr$", new FunctionDefinition {Arity = 1, Call = (i, a) => i.Functions.Chr(a[0])}},
            {"int", new FunctionDefinition {Arity = 1, Call = (i, a) => i.Functions.Int(i, a[0])}},
            {"i.", new FunctionDefinition {Arity = 1, Call = (i, a) => i.Functions.Int(i, a[0])}},
            {"mem", new FunctionDefinition {Arity = 0, Call = (i, a) => i.Functions.Mem(i)}},
            {"m.", new FunctionDefinition {Arity = 0, Call = (i, a) => i.Functions.Mem(i)}},
            {"point", new FunctionDefinition {Arity = 2, Call = (i, a) => i.Functions.Point(i, a[0], a[1])}},
            {"rnd", new FunctionDefinition {Arity = 1, Call = (i, a) => i.Functions.Rnd(a[0])}},
            {"reset", new FunctionDefinition {Arity = 2, Call = (i, a) => i.Functions.Reset(i, a[0], a[1])}},
            {"set", new FunctionDefinition {Arity = 2, Call = (i, a) => i.Functions.Set(i, a[0], a[1])}},
        };
    }

    public FunctionDefinition Get(string name)
    {
        string lowerName = name.ToLower();
        return _functions.ContainsKey(lowerName) ? _functions[lowerName] : null;
    }
}