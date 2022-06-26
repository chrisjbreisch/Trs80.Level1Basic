using System;

namespace Trs80.Level1Basic.VirtualMachine.Exceptions;

public class ProgramTooLargeException : BaseException
{

    public ProgramTooLargeException(int lineNumber, string statement, int linePosition, string message) :
        base(lineNumber, statement, linePosition, message)
    {

    }
}