using System;

using Trs80.Level1Basic.VirtualMachine.Scanner;

namespace Trs80.Level1Basic.VirtualMachine.Exceptions;

public class RuntimeExpressionException : BaseException
{
    public RuntimeExpressionException(int lineNumber, string statement, int linePosition, string message) : 
        base(lineNumber, statement, linePosition, message) 
    {
    }
}