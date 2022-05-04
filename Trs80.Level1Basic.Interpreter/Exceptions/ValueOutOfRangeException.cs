using System;

namespace Trs80.Level1Basic.Interpreter.Exceptions;

public class ValueOutOfRangeException : Exception
{
    public int LineNumber { get; }
    public string Statement { get; }
    public ValueOutOfRangeException(int lineNumber, string statement, string message) : base(message)
    {
        LineNumber = lineNumber;
        Statement = statement;
    }
}