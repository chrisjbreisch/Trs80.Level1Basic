using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Trs80.Level1Basic.VirtualMachine.Machine;

[SuppressMessage("ReSharper", "UnusedParameter.Local")]
public class NativeFunctions : INativeFunctions
{
    private readonly Dictionary<string, List<Callable>> _functions;

    public NativeFunctions()
    {
        _functions = new Dictionary<string, List<Callable>>
        {
            {"_pad_quadrant", new List<Callable> { new() {Arity = 0, Call = (api, arg) => api.PadQuadrant()}}},
            {"abs", new List<Callable> { new()  {Arity = 1, Call = (api, arg) => api.Abs(arg[0])}}},
            {"a.", new List<Callable> { new()  {Arity = 1, Call = (api, arg) => api.Abs(arg[0])}}},
            {"chr$", new List<Callable> { new()  {Arity = 1, Call = (api, arg) => api.Chr(arg[0])}}},
            {"int", new List<Callable> { new()  {Arity = 1, Call = (api, arg) => api.Int(arg[0])}}},
            {"i.", new List<Callable> { new()  {Arity = 1, Call = (api, arg) => api.Int(arg[0])}}},
            {"mem", new List<Callable> { new()  {Arity = 0, Call = (api, arg) => api.Mem()}}},
            {"m.", new List<Callable> { new()  {Arity = 0, Call = (api, arg) => api.Mem()}}},
            {"point", new List<Callable> { new()  {Arity = 2, Call = (api, arg) => api.Point(arg[0], arg[1])}}},
            {"p.", new List<Callable> { new()  {Arity = 2, Call = (api, arg) => api.Point(arg[0], arg[1])}}},
            {"r.", new List<Callable> {
                new()  {Arity = 1, Call = (api, arg) => api.Rnd(arg[0])},
                new()  {Arity = 2, Call = (api, arg) => api.Reset(arg[0], arg[1])}
            }},
            {"rnd", new List<Callable> { new()  {Arity = 1, Call = (api, arg) => api.Rnd(arg[0])}}},
            {"reset", new List<Callable> { new()  {Arity = 2, Call = (api, arg) => api.Reset(arg[0], arg[1])}}},
            {"s.", new List<Callable> { new()  {Arity = 2, Call = (api, arg) => api.Set(arg[0], arg[1])}}},
            {"set", new List<Callable> { new()  {Arity = 2, Call = (api, arg) => api.Set(arg[0], arg[1])}}},
            {"tab", new List<Callable> { new()  {Arity = 1, Call = (api, arg) => api.Tab(arg[0])}}},
            {"t.", new List<Callable> { new()  {Arity = 1, Call = (api, arg) => api.Tab(arg[0])}}},
        };
    }

    public List<Callable> Get(string name)
    {
        string lowerName = name.ToLower();
        return _functions.ContainsKey(lowerName) ? _functions[lowerName] : null;
    }
}