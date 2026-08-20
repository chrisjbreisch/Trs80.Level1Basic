using System;

using Trs80.Level1Basic.Common;
using Trs80.Level1Basic.VirtualMachine.Exceptions;
using Trs80.Level1Basic.VirtualMachine.Interpreter;

namespace Trs80.Level1Basic.VirtualMachine.Machine;

public class Trs80Api : ITrs80Api
{
    private const int MemorySize = 64 * 1024;
    private static readonly byte[] Memory = new byte[MemorySize];
    private readonly IProgram _program;
    private readonly IMachine _machine;
    private readonly ITrs80 _trs80;
    private readonly BasicLanguageLevel _basicLevel;
    public const int AdditionalMem = 12 * 1024 - 300;
    public const int BaseMem = 3591;
    public const int TotalMemory = BaseMem + AdditionalMem;
    public const int MaxArrayIndex = 3963;
    public int GetMaxArrayIndex() => MaxArrayIndex - _machine.VariableMemorySize / 7;

    public Trs80Api(IProgram program, IMachine machine, ITrs80 trs80, BasicLanguageLevel basicLevel)
    {
        _program = program ?? throw new ArgumentNullException(nameof(program));
        _machine = machine ?? throw new ArgumentNullException(nameof(machine));
        _trs80 = trs80 ?? throw new ArgumentNullException(nameof(trs80));
        _basicLevel = basicLevel;
    }

    public int Int(dynamic value)
    {
        return (int)Math.Floor(Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture));
    }


    public dynamic Mem()
    {
        return TotalMemory - _program.Size() - _machine.VariableMemorySize;
    }

    public dynamic Fre(dynamic value)
    {
        return Mem();
    }

    public dynamic Abs(dynamic value)
    {
        if (value is float fValue)
            return Math.Abs(fValue);
        if (value is double dValue)
            return Math.Abs(dValue);

        return Math.Abs((int)value);
    }

    public dynamic Chr(dynamic value)
    {
        int characterCode = (int)Math.Truncate(Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture));
        if (characterCode is < 0 or > byte.MaxValue)
            throw new ValueOutOfRangeException(-1, string.Empty, "Character code out of range.");

        return (char)characterCode;
    }

    public int Asc(dynamic value)
    {
        if (value is string s && s.Length > 0)
            return s[0];

        if (value is char c)
            return c;

        return 0;
    }

    public int Len(dynamic value)
    {
        if (value is string s)
            return s.Length;

        return value?.ToString().Length ?? 0;
    }

    public int InStr(string source, string match)
    {
        return InStr(1, source, match);
    }

    public int InStr(int start, string source, string match)
    {
        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(match))
            return 0;

        int searchStart = Math.Max(start, 1) - 1;
        if (searchStart >= source.Length)
            return 0;

        int index = source.IndexOf(match, searchStart, StringComparison.OrdinalIgnoreCase);
        return index < 0 ? 0 : index + 1;
    }

    public float Val(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return 0;

        string text = value.Trim();
        for (int length = text.Length; length > 0; length--)
        {
            string candidate = text[..length];
            if (float.TryParse(candidate, System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, out float numericValue))
                return numericValue;
        }

        return 0;
    }

    public string Str(dynamic value)
    {
        if (value is null)
            return string.Empty;

        return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
    }

    public int CInt(dynamic value)
    {
        double numericValue = Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture);
        double roundedValue = Math.Round(numericValue, MidpointRounding.AwayFromZero);
        bool isDoublePrecision = value is double;
        double minimum = isDoublePrecision ? int.MinValue : short.MinValue;
        double maximum = isDoublePrecision ? int.MaxValue : short.MaxValue;
        if (roundedValue < minimum || roundedValue > maximum)
            throw new ValueOutOfRangeException(-1, string.Empty, "Integer value out of range.");
        return (int)roundedValue;
    }

    public int Cvi(string value)
    {
        if (string.IsNullOrEmpty(value) || value.Length < 2)
            return 0;

        return (short)((byte)value[0] | ((byte)value[1] << 8));
    }

    public float Cvs(string value)
    {
        if (string.IsNullOrEmpty(value) || value.Length < 4)
            return 0;

        byte[] bytes = { (byte)value[0], (byte)value[1], (byte)value[2], (byte)value[3] };
        if (!BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        return BitConverter.ToSingle(bytes, 0);
    }

    public double Cvd(string value)
    {
        if (string.IsNullOrEmpty(value) || value.Length < 8)
            return 0;

        byte[] bytes = {
            (byte)value[0], (byte)value[1], (byte)value[2], (byte)value[3],
            (byte)value[4], (byte)value[5], (byte)value[6], (byte)value[7]
        };
        if (!BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        return BitConverter.ToDouble(bytes, 0);
    }

    public string Mki(dynamic value)
    {
        short number = Convert.ToInt16(value, System.Globalization.CultureInfo.InvariantCulture);
        return new string(new[]
        {
            (char)(byte)number,
            (char)(byte)(number >> 8)
        });
    }

    public string Mks(dynamic value)
    {
        byte[] bytes = BitConverter.GetBytes(Convert.ToSingle(value, System.Globalization.CultureInfo.InvariantCulture));
        if (!BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        return new string(new[]
        {
            (char)bytes[0],
            (char)bytes[1],
            (char)bytes[2],
            (char)bytes[3]
        });
    }

    public string Mkd(dynamic value)
    {
        byte[] bytes = BitConverter.GetBytes(Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture));
        if (!BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        return new string(new[]
        {
            (char)bytes[0],
            (char)bytes[1],
            (char)bytes[2],
            (char)bytes[3],
            (char)bytes[4],
            (char)bytes[5],
            (char)bytes[6],
            (char)bytes[7]
        });
    }

    public int Fix(dynamic value)
    {
        double numericValue = Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture);
        return (int)Math.Truncate(numericValue);
    }

    public double CDbl(dynamic value)
    {
        double numericValue = Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture);
        if (Math.Abs(numericValue) > 1.701411834544556E+38)
            throw new ValueOutOfRangeException(-1, string.Empty, "Double value out of range.");

        return numericValue;
    }

    public float CSng(dynamic value)
    {
        double numericValue = Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture);
        if (Math.Abs(numericValue) > 1.701411E+38)
            throw new ValueOutOfRangeException(-1, string.Empty, "Single value out of range.");

        return (float)numericValue;
    }

    public string LCase(string value)
    {
        return value is null ? string.Empty : value.ToLowerInvariant();
    }

    public string UCase(string value)
    {
        return value is null ? string.Empty : value.ToUpperInvariant();
    }

    public string Trim(string value)
    {
        return value is null ? string.Empty : value.Trim();
    }

    public string LTrim(string value)
    {
        return value is null ? string.Empty : value.TrimStart();
    }

    public string RTrim(string value)
    {
        return value is null ? string.Empty : value.TrimEnd();
    }

    public int Peek(int address)
    {
        return Memory[NormalizeAddress(address)];
    }

    public void Poke(int address, int value)
    {
        Memory[NormalizeAddress(address)] = (byte)(value & 0xFF);
    }

    private static int NormalizeAddress(int address)
    {
        return ((address % MemorySize) + MemorySize) % MemorySize;
    }

    public int Pos(int position)
    {
        return _trs80.CursorX;
    }

    public int CsrLin()
    {
        return _trs80.CursorY;
    }

    public string InKey()
    {
        if (!_trs80.TryReadKey(out ConsoleKeyInfo key) || key.KeyChar == '\0')
            return string.Empty;

        return key.KeyChar.ToString().ToUpperInvariant();
    }

    public string InputString(int length)
    {
        if (length <= 0)
            return string.Empty;

        char[] buffer = new char[length];
        int charsRead = _trs80.In.ReadBlock(buffer, 0, length);
        if (charsRead <= 0)
            return string.Empty;

        return new string(buffer, 0, charsRead).ToUpperInvariant();
    }

    public string Date()
    {
        return DateTime.Now.ToString("MM/dd/yy");
    }

    public string Time()
    {
        return DateTime.Now.ToString("HH:mm:ss");
    }

    public double Sqr(dynamic value)
    {
        return Math.Sqrt(Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture));
    }

    public double Sin(dynamic value)
    {
        return Math.Sin(Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture));
    }

    public double Cos(dynamic value)
    {
        return Math.Cos(Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture));
    }

    public double Tan(dynamic value)
    {
        return Math.Tan(Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture));
    }

    public double Atn(dynamic value)
    {
        return Math.Atan(Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture));
    }

    public double Log(dynamic value)
    {
        return Math.Log(Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture));
    }

    public double Exp(dynamic value)
    {
        return Math.Exp(Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture));
    }

    public int Sgn(dynamic value)
    {
        if (value is null)
            return 0;

        float numericValue = Convert.ToSingle(value, System.Globalization.CultureInfo.InvariantCulture);
        if (numericValue < 0)
            return -1;

        if (numericValue > 0)
            return 1;

        return 0;
    }

    public string Hex(dynamic value)
    {
        int number = (int)Math.Floor((float)value);
        return Convert.ToString(number, 16).ToUpperInvariant();
    }

    public string Oct(dynamic value)
    {
        int number = (int)Math.Floor((float)value);
        return Convert.ToString(number, 8);
    }

    public string String(int count, dynamic value)
    {
        if (count <= 0 || value is null)
            return string.Empty;

        char character = value is string text
            ? string.IsNullOrEmpty(text) ? '\0' : text[0]
            : GetCharacterCode(value);

        return character == '\0' ? string.Empty : new string(character, count);
    }

    private static char GetCharacterCode(dynamic value)
    {
        int characterCode = (int)Math.Truncate(Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture));
        if (characterCode is < 0 or > byte.MaxValue)
            throw new ValueOutOfRangeException(-1, string.Empty, "Character code out of range.");

        return (char)characterCode;
    }

    public string Space(int length)
    {
        return length <= 0 ? string.Empty : new string(' ', length);
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

    public string Mid(string value, int start)
    {
        return Mid(value, start, value?.Length ?? 0);
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

    public string MidAssign(string value, int start, int length, string replacement)
    {
        if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(replacement) || start <= 0 || length <= 0)
            return value ?? string.Empty;

        int zeroBasedStart = start - 1;
        if (zeroBasedStart >= value.Length)
            return value;

        char[] result = value.ToCharArray();
        int replacementLength = Math.Min(length, replacement.Length);
        replacement.CopyTo(0, result, zeroBasedStart, Math.Min(replacementLength, result.Length - zeroBasedStart));
        return new string(result);
    }

    public static Random Rand = new();
    private static float LastRandom;
    private static bool HasLastRandom;

    public dynamic Rnd(dynamic control)
    {
        int integerControl = control is null
            ? 0
            : (int)Convert.ToSingle(control, System.Globalization.CultureInfo.InvariantCulture);

        if (integerControl < 0)
        {
            Rand = new Random(integerControl);
            LastRandom = (float)Rand.NextDouble();
            HasLastRandom = true;
            return LastRandom;
        }

        if (integerControl == 0)
        {
            if (HasLastRandom)
                return LastRandom;

            LastRandom = (float)Rand.NextDouble();
            HasLastRandom = true;
            return LastRandom;
        }

        LastRandom = (int)Math.Floor(integerControl * Rand.NextDouble() + 1);
        HasLastRandom = true;
        return LastRandom;
    }

    public static void ResetRng()
    {
        Rand = new Random();
        LastRandom = 0;
        HasLastRandom = false;
    }

    public string Tab(dynamic value)
    {
        int position = (int)Convert.ToSingle(value, System.Globalization.CultureInfo.InvariantCulture);
        return _trs80.PadToPosition(position);
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