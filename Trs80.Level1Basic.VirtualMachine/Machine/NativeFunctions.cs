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
            {"asc", new List<Callable> { new() {Arity = 1, Call = (api, arg) => api.Asc(arg[0])}}},
            {"chr$", new List<Callable> { new()  {Arity = 1, Call = (api, arg) => api.Chr(arg[0])}}},
            {"cint", new List<Callable> { new() {Arity = 1, Call = (api, arg) => api.CInt(arg[0])}}},
            {"cdbl", new List<Callable> { new() {Arity = 1, Call = (api, arg) => api.CDbl(arg[0])}}},
            {"csng", new List<Callable> { new() {Arity = 1, Call = (api, arg) => api.CSng(arg[0])}}},
            {"fix", new List<Callable> { new() {Arity = 1, Call = (api, arg) => api.Fix(arg[0])}}},
            {"int", new List<Callable> { new()  {Arity = 1, Call = (api, arg) => api.Int(arg[0])}}},
            {"i.", new List<Callable> { new()  {Arity = 1, Call = (api, arg) => api.Int(arg[0])}}},
            {"hex$", new List<Callable> { new() {Arity = 1, Call = (api, arg) => api.Hex(arg[0])}}},
            {"lcase$", new List<Callable> { new() {Arity = 1, Call = (api, arg) => api.LCase((string)arg[0])}}},
            {"ltrim$", new List<Callable> { new() {Arity = 1, Call = (api, arg) => api.LTrim((string)arg[0])}}},
            {"trim$", new List<Callable> { new() {Arity = 1, Call = (api, arg) => api.Trim((string)arg[0])}}},
            {"rtrim$", new List<Callable> { new() {Arity = 1, Call = (api, arg) => api.RTrim((string)arg[0])}}},
            {"peek", new List<Callable> { new() {Arity = 1, Call = (api, arg) => api.Peek((int)arg[0])}}},
            {"poke", new List<Callable> { new() {Arity = 2, Call = (api, arg) => { api.Poke((int)arg[0], (int)arg[1]); return null; }}}},
            {"pos", new List<Callable> { new() {Arity = 1, Call = (api, arg) => api.Pos((int)arg[0])}}},
            {"csrlin", new List<Callable> { new() {Arity = 0, Call = (api, arg) => api.CsrLin()}}},
            {"inkey$", new List<Callable> { new() {Arity = 0, Call = (api, arg) => api.InKey()}}},
            {"input$", new List<Callable> { new() {Arity = 1, Call = (api, arg) => api.InputString((int)arg[0])}}},
            {"sqr", new List<Callable> { new() {Arity = 1, Call = (api, arg) => api.Sqr(arg[0])}}},
            {"sin", new List<Callable> { new() {Arity = 1, Call = (api, arg) => api.Sin(arg[0])}}},
            {"cos", new List<Callable> { new() {Arity = 1, Call = (api, arg) => api.Cos(arg[0])}}},
            {"tan", new List<Callable> { new() {Arity = 1, Call = (api, arg) => api.Tan(arg[0])}}},
            {"atn", new List<Callable> { new() {Arity = 1, Call = (api, arg) => api.Atn(arg[0])}}},
            {"log", new List<Callable> { new() {Arity = 1, Call = (api, arg) => api.Log(arg[0])}}},
            {"exp", new List<Callable> { new() {Arity = 1, Call = (api, arg) => api.Exp(arg[0])}}},
            {"date$", new List<Callable> { new() {Arity = 0, Call = (api, arg) => api.Date()}}},
            {"time$", new List<Callable> { new() {Arity = 0, Call = (api, arg) => api.Time()}}},
            {"ucase$", new List<Callable> { new() {Arity = 1, Call = (api, arg) => api.UCase((string)arg[0])}}},
            {"instr", new List<Callable> {
                new() {Arity = 2, Call = (api, arg) => api.InStr((string)arg[0], (string)arg[1])},
                new() {Arity = 3, Call = (api, arg) => api.InStr((int)arg[0], (string)arg[1], (string)arg[2])}
            }},
            {"left$", new List<Callable> { new() {Arity = 2, Call = (api, arg) => api.Left((string)arg[0], (int)arg[1])}}},
            {"len", new List<Callable> { new() {Arity = 1, Call = (api, arg) => api.Len(arg[0])}}},
            {"mid$", new List<Callable> { new() {Arity = 3, Call = (api, arg) => api.Mid((string)arg[0], (int)arg[1], (int)arg[2])}}},
            {"mem", new List<Callable> { new()  {Arity = 0, Call = (api, arg) => api.Mem()}}},
            {"fre", new List<Callable> { new() {Arity = 1, Call = (api, arg) => api.Fre(arg[0])}}},
            {"oct$", new List<Callable> { new() {Arity = 1, Call = (api, arg) => api.Oct(arg[0])}}},
            {"sgn", new List<Callable> { new() {Arity = 1, Call = (api, arg) => api.Sgn(arg[0])}}},
            {"spc", new List<Callable> { new() {Arity = 1, Call = (api, arg) => api.Space((int)arg[0])}}},
            {"space$", new List<Callable> { new() {Arity = 1, Call = (api, arg) => api.Space((int)arg[0])}}},
            {"str$", new List<Callable> { new() {Arity = 1, Call = (api, arg) => api.Str(arg[0])}}},
            {"string$", new List<Callable> { new() {Arity = 2, Call = (api, arg) => api.String((int)arg[0], arg[1])}}},
            {"m.", new List<Callable> { new()  {Arity = 0, Call = (api, arg) => api.Mem()}}},
            {"point", new List<Callable> { new()  {Arity = 2, Call = (api, arg) => api.Point(arg[0], arg[1])}}},
            {"p.", new List<Callable> { new()  {Arity = 2, Call = (api, arg) => api.Point(arg[0], arg[1])}}},
            {"r.", new List<Callable> {
                new()  {Arity = 0, Call = (api, arg) => api.Rnd(0)},
                new()  {Arity = 1, Call = (api, arg) => api.Rnd(arg[0])},
                new()  {Arity = 2, Call = (api, arg) => api.Reset(arg[0], arg[1])}
            }},
            {"right$", new List<Callable> { new() {Arity = 2, Call = (api, arg) => api.Right((string)arg[0], (int)arg[1])}}},
            {"rnd", new List<Callable> {
                new()  {Arity = 0, Call = (api, arg) => api.Rnd(0)},
                new()  {Arity = 1, Call = (api, arg) => api.Rnd(arg[0])}
            }},
            {"val", new List<Callable> { new() {Arity = 1, Call = (api, arg) => api.Val((string)arg[0])}}},
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