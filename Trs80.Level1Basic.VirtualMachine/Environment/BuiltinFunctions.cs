using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Trs80.Level1Basic.VirtualMachine.Environment;

public interface IBuiltinFunctions
{
    List<FunctionDefinition> Get(string name);
}

[SuppressMessage("ReSharper", "UnusedParameter.Local")]
public class BuiltinFunctions : IBuiltinFunctions
{
    private readonly Dictionary<string, List<FunctionDefinition>> _functions;

    public BuiltinFunctions()
    {
        _functions = new Dictionary<string, List<FunctionDefinition>>
        {
            {"_padquadrant", new List<FunctionDefinition> { new() {Arity = 0, Call = (m, a) => m.PadQuadrant()}}},
            {"abs", new List<FunctionDefinition> { new()  {Arity = 1, Call = (m, a) => m.Abs(a[0])}}},
            {"a.", new List<FunctionDefinition> { new()  {Arity = 1, Call = (m, a) => m.Abs(a[0])}}},
            {"chr$", new List<FunctionDefinition> { new()  {Arity = 1, Call = (m, a) => m.Chr(a[0])}}},
            {"int", new List<FunctionDefinition> { new()  {Arity = 1, Call = (m, a) => m.Int(a[0])}}},
            {"i.", new List<FunctionDefinition> { new()  {Arity = 1, Call = (m, a) => m.Int(a[0])}}},
            {"mem", new List<FunctionDefinition> { new()  {Arity = 0, Call = (m, a) => m.Mem()}}},
            {"m.", new List<FunctionDefinition> { new()  {Arity = 0, Call = (m, a) => m.Mem()}}},
            {"point", new List<FunctionDefinition> { new()  {Arity = 2, Call = (m, a) => m.Point(a[0], a[1])}}},
            {"p.", new List<FunctionDefinition> { new()  {Arity = 2, Call = (m, a) => m.Point(a[0], a[1])}}},
            {"r.", new List<FunctionDefinition> {
                new()  {Arity = 1, Call = (m, a) => m.Rnd(a[0])},
                new()  {Arity = 2, Call = (m, a) => m.Reset(a[0], a[1])}
            }},
            {"rnd", new List<FunctionDefinition> { new()  {Arity = 1, Call = (m, a) => m.Rnd(a[0])}}},
            {"reset", new List<FunctionDefinition> { new()  {Arity = 2, Call = (m, a) => m.Reset(a[0], a[1])}}},
            {"s.", new List<FunctionDefinition> { new()  {Arity = 2, Call = (m, a) => m.Set(a[0], a[1])}}},
            {"set", new List<FunctionDefinition> { new()  {Arity = 2, Call = (m, a) => m.Set(a[0], a[1])}}},
            {"tab", new List<FunctionDefinition> { new()  {Arity = 1, Call = (m, a) => m.Tab(a[0])}}},
            {"t.", new List<FunctionDefinition> { new()  {Arity = 1, Call = (m, a) => m.Tab(a[0])}}},
        };
    }

    public List<FunctionDefinition> Get(string name)
    {
        string lowerName = name.ToLower();
        return _functions.ContainsKey(lowerName) ? _functions[lowerName] : null;
    }
}