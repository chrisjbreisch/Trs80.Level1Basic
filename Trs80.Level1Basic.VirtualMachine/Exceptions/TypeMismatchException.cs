namespace Trs80.Level1Basic.VirtualMachine.Exceptions;

public class TypeMismatchException : BaseException
{
    public TypeMismatchException(int lineNumber, string statement, int linePosition, string message) :
        base(lineNumber, statement, linePosition, message)
    {
    }
}
