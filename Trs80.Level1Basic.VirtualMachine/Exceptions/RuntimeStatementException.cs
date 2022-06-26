using System;

namespace Trs80.Level1Basic.VirtualMachine.Exceptions;

public class RuntimeStatementException : BaseException
{
    public RuntimeStatementException(int lineNumber, string statement, int linePosition, string message) : 
        base(lineNumber, statement, linePosition, message)
    {
    }
}