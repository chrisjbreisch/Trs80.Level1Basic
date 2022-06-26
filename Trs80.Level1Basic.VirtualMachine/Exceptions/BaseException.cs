using System;
using Trs80.Level1Basic.VirtualMachine.Parser.Statements;

namespace Trs80.Level1Basic.VirtualMachine.Exceptions;

public abstract class BaseException : Exception
{
    public int LineNumber { get; }
    public string Statement { get; }
    public int LinePosition { get; }
    protected BaseException(int lineNumber, string statement, int linePosition, string message) : base(message)
    {
        LineNumber = lineNumber;
        Statement = statement;
        LinePosition = linePosition;
    }
}