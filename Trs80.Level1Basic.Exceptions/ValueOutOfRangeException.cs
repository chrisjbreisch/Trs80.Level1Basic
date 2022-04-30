using System;

namespace Trs80.Level1Basic.Exceptions;

public class ValueOutOfRangeException : Exception
{
    public int LineNumber { get; }
    public string SourceLine { get;  }
    public ValueOutOfRangeException(int lineNumber, string sourceLine, string message) : base(message)
    {
        LineNumber = lineNumber;
        SourceLine = sourceLine;
    }
}