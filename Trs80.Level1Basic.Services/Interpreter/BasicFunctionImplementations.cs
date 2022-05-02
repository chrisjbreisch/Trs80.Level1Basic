using System;
using Trs80.Level1Basic.Exceptions;

namespace Trs80.Level1Basic.Services.Interpreter;

public class BasicFunctionImplementations
{
    public int Int(IBasicInterpreter interpreter, dynamic value)
    {
        if (Math.Abs(value) > short.MaxValue)
            throw new ValueOutOfRangeException(interpreter.CurrentStatement.LineNumber, interpreter.CurrentStatement.SourceLine,
                $"{value} is out of range for int().");
        return (int)Math.Floor((float)value);
    }

    public string Tab(IBasicInterpreter interpreter, dynamic value)
    {
        interpreter.WriteToPosition(value);

        return string.Empty;
    }

    public dynamic Mem(IBasicInterpreter interpreter)
    {
        return 3583 + 12 * 1024 - interpreter.MemoryInUse();
    }

    public dynamic Abs(dynamic value)
    {
        if (value is float fValue)
            return Math.Abs(fValue);
        return Math.Abs((int) value);
    }

    public dynamic Chr(dynamic value)
    {
        return (char)value;
    }

    private static readonly Random Rand = new();
    public dynamic Rnd(int control)
    {
        if (control == 0)
            return (float) Rand.NextDouble();

        return (int) Math.Floor(control * Rand.NextDouble() + 1);
    }

    public string PadQuadrant(IBasicInterpreter interpreter)
    {
        return interpreter.PadQuadrant();
    }

    public object Set(IBasicInterpreter interpreter, float x, float y)
    {
        interpreter.Set((int)x, (int)y);
        return null;
    }

    public object Reset(IBasicInterpreter interpreter, float x, float y)
    {
        interpreter.Reset((int)x, (int)y);
        return null;

    }

    public bool Point(IBasicInterpreter interpreter, int x, int y)
    {
        return interpreter.Point(x, y);// ? 1 : 0;
    }
}