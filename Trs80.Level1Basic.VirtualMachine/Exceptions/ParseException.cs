using System;

namespace Trs80.Level1Basic.VirtualMachine.Exceptions;

public class ParseException : Exception
{
    public int LineNumber { get; }
    public string Statement { get; }
    public ParseException(int lineNumber, string statement, string message) : base(message)
    {
        LineNumber = lineNumber;
        Statement = statement;
    }
}