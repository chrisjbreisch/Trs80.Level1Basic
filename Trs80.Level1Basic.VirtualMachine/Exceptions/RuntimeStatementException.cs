using System;

namespace Trs80.Level1Basic.VirtualMachine.Exceptions;

public class RuntimeStatementException : Exception
{
    public int LineNumber { get; }
    public string Statement { get; }

    public RuntimeStatementException(int lineNumber, string statement, string message) : base(message)
    {
        LineNumber = lineNumber;
        Statement = statement;
    }

    public RuntimeStatementException(int lineNumber, string statement, string message, Exception innerException) : base(message, innerException)
    {
        LineNumber = lineNumber;
        Statement = statement;
    }

}