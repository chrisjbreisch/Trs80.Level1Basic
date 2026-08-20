namespace Trs80.Level1Basic.VirtualMachine.Exceptions;

public class UndefinedLineException : BaseException
{
    public UndefinedLineException(int lineNumber, string statement, int linePosition, string message)
        : base(lineNumber, statement, linePosition, message)
    {
    }
}