using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Trs80.Level1Basic.VirtualMachine.Machine;

public interface INativeFunctions
{
    List<Callable> Get(string name);
}

[SuppressMessage("ReSharper", "UnusedParameter.Local")]
public class NativeFunctions : INativeFunctions
{
    private readonly Dictionary<string, List<Callable>> _functions;

    public NativeFunctions()
    {
        _functions = new Dictionary<string, List<Callable>>
        {
            {"_padquadrant", new List<Callable> { new() {Arity = 0, Call = (m, a) => m.PadQuadrant()}}},
            {"abs", new List<Callable> { new()  {Arity = 1, Call = (m, a) => m.Abs(a[0])}}},
            {"a.", new List<Callable> { new()  {Arity = 1, Call = (m, a) => m.Abs(a[0])}}},
            {"chr$", new List<Callable> { new()  {Arity = 1, Call = (m, a) => m.Chr(a[0])}}},
            {"int", new List<Callable> { new()  {Arity = 1, Call = (m, a) => m.Int(a[0])}}},
            {"i.", new List<Callable> { new()  {Arity = 1, Call = (m, a) => m.Int(a[0])}}},
            {"mem", new List<Callable> { new()  {Arity = 0, Call = (m, a) => m.Mem()}}},
            {"m.", new List<Callable> { new()  {Arity = 0, Call = (m, a) => m.Mem()}}},
            {"point", new List<Callable> { new()  {Arity = 2, Call = (m, a) => m.Point(a[0], a[1])}}},
            {"p.", new List<Callable> { new()  {Arity = 2, Call = (m, a) => m.Point(a[0], a[1])}}},
            {"r.", new List<Callable> {
                new()  {Arity = 1, Call = (m, a) => m.Rnd(a[0])},
                new()  {Arity = 2, Call = (m, a) => m.Reset(a[0], a[1])}
            }},
            {"rnd", new List<Callable> { new()  {Arity = 1, Call = (m, a) => m.Rnd(a[0])}}},
            {"reset", new List<Callable> { new()  {Arity = 2, Call = (m, a) => m.Reset(a[0], a[1])}}},
            {"s.", new List<Callable> { new()  {Arity = 2, Call = (m, a) => m.Set(a[0], a[1])}}},
            {"set", new List<Callable> { new()  {Arity = 2, Call = (m, a) => m.Set(a[0], a[1])}}},
            {"tab", new List<Callable> { new()  {Arity = 1, Call = (m, a) => m.Tab(a[0])}}},
            {"t.", new List<Callable> { new()  {Arity = 1, Call = (m, a) => m.Tab(a[0])}}},
        };
    }

    public List<Callable> Get(string name)
    {
        string lowerName = name.ToLower();
        return _functions.ContainsKey(lowerName) ? _functions[lowerName] : null;
    }
}