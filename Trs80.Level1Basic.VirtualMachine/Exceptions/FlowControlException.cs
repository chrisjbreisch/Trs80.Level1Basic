namespace Trs80.Level1Basic.VirtualMachine.Exceptions;

public class FlowControlException : BaseException
{
    public FlowControlException(int lineNumber, string statement, int linePosition, string message)
        : base(lineNumber, statement, linePosition, message)
    {
    }
}