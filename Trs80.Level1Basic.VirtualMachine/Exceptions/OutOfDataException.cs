namespace Trs80.Level1Basic.VirtualMachine.Exceptions;

public class OutOfDataException : BaseException
{
    public OutOfDataException(int lineNumber, string statement, int linePosition, string message)
        : base(lineNumber, statement, linePosition, message)
    {
    }
}