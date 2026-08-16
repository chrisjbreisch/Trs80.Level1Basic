using System;

namespace Trs80.Level1Basic.Common;

public sealed class AutoLineNumbering
{
    private int _nextLineNumber;
    private int _increment;

    public bool IsActive { get; private set; }

    public void Start(int firstLineNumber = 10, int increment = 10)
    {
        if (increment <= 0)
            throw new ArgumentOutOfRangeException(nameof(increment));

        _nextLineNumber = firstLineNumber;
        _increment = increment;
        IsActive = true;
    }

    public bool TryNumber(string sourceLine, out string numberedLine)
    {
        if (!IsActive || string.IsNullOrWhiteSpace(sourceLine))
        {
            numberedLine = string.Empty;
            return false;
        }

        numberedLine = $"{_nextLineNumber} {sourceLine}";
        _nextLineNumber += _increment;
        return true;
    }

    public void Stop()
    {
        IsActive = false;
    }
}