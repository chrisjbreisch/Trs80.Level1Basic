namespace Trs80.Level1Basic.VirtualMachine.Exceptions;

public class ParseException : BaseException
{
    public ParseException(int lineNumber, string statement, int linePosition, string message) :
        base(lineNumber, statement, linePosition, message)
    {
    }
}