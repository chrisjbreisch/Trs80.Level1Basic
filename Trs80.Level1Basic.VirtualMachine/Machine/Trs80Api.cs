using System;

using Trs80.Level1Basic.VirtualMachine.Interpreter;

namespace Trs80.Level1Basic.VirtualMachine.Machine;

public class Trs80Api : ITrs80Api
{
    private readonly IProgram _program;
    private readonly ITrs80 _trs80;
    public const int AdditionalMem = 12 * 1024;
    public const int BaseMem = 3583;
    public const int TotalMemory = BaseMem + AdditionalMem;

    public Trs80Api(IProgram program, ITrs80 trs80)
    {
        _program = program ?? throw new ArgumentNullException(nameof(program));
        _trs80 = trs80 ?? throw new ArgumentNullException(nameof(trs80));
    }

    public int Int(dynamic value)
    {
        return (int)Math.Floor((float)value);
    }


    public dynamic Mem()
    {
        return TotalMemory - _program.Size();
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

    public string Left(string value, int length)
    {
        if (string.IsNullOrEmpty(value) || length <= 0)
            return string.Empty;

        return value.Length <= length ? value : value[..length];
    }

    public string Right(string value, int length)
    {
        if (string.IsNullOrEmpty(value) || length <= 0)
            return string.Empty;

        int start = value.Length - length;
        return start <= 0 ? value : value[start..];
    }

    public string Mid(string value, int start, int length)
    {
        if (string.IsNullOrEmpty(value) || start <= 0 || length <= 0)
            return string.Empty;

        int zeroBasedStart = start - 1;
        if (zeroBasedStart >= value.Length)
            return string.Empty;

        int end = Math.Min(value.Length, zeroBasedStart + length);
        return value[zeroBasedStart..end];
    }

    public static readonly Random Rand = new();

    public dynamic Rnd(int control)
    {
        if (control == 0)
            return (float)Rand.NextDouble();

        return (int)Math.Floor(control * Rand.NextDouble() + 1);
    }

    public string Tab(dynamic value)
    {
        return _trs80.PadToPosition(value);
    }

    public string PadQuadrant()
    {
        return _trs80.PadQuadrant();
    }

    public object Set(float x, float y)
    {
        return _trs80.Set(x, y);
    }

    public object Reset(float x, float y)
    {
        return _trs80.Reset(x, y);
    }

    public int Point(int x, int y)
    {
        return _trs80.Point(x, y);
    }
}