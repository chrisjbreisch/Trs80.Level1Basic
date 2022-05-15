using System;

using Trs80.Level1Basic.Console;
using Trs80.Level1Basic.VirtualMachine.Exceptions;

namespace Trs80.Level1Basic.VirtualMachine.Environment;

public interface IMachine
{
    int Int(dynamic value);
    dynamic Mem();
    dynamic Abs(dynamic value);
    dynamic Chr(dynamic value);
    dynamic Rnd(int control);
    string Tab(dynamic value);
    string PadQuadrant();
    object Set(float x, float y);
    object Reset(float x, float y);
    int Point(int x, int y);
}

public class Machine : IMachine
{
    private readonly IEnvironment _environment;
    private readonly IConsole _console;

    public Machine(IEnvironment environment, IConsole console)
    {
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        _console = console ?? throw new ArgumentNullException(nameof(console));
    }

    public int Int(dynamic value)
    {
        if (Math.Abs(value) > short.MaxValue)
            throw new ValueOutOfRangeException(_environment.CurrentStatement.LineNumber, _environment.CurrentStatement.SourceLine,
                $"{value} is out of range for int().");
        return (int)Math.Floor((float)value);
    }

    private const int TotalMemory = 3583 + 12 * 1024;
    public dynamic Mem()
    {
        return TotalMemory - _environment.MemoryInUse();
    }

    public dynamic Abs(dynamic value)
    {
        if (value is float fValue)
            return Math.Abs(fValue);
        return Math.Abs((int)value);
    }

    public dynamic Chr(dynamic value)
    {
        return (char)value;
    }

    private static readonly Random Rand = new();
    public dynamic Rnd(int control)
    {
        if (control == 0)
            return (float)Rand.NextDouble();

        return (int)Math.Floor(control * Rand.NextDouble() + 1);
    }

    public string Tab(dynamic value)
    {
        return _console.PadToPosition(value);
    }

    public string PadQuadrant()
    {
        return _console.PadToQuadrant();
    }

    public object Set(float x, float y)
    {
        _console.Set((int)x, (int)y);
        return null;
    }

    public object Reset(float x, float y)
    {
        _console.Reset((int)x, (int)y);
        return null;

    }

    public int Point(int x, int y)
    {
        return _console.Point(x, y) ? 1 : 0;
    }
}