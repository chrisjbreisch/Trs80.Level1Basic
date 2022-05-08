using System;

namespace Trs80.Level1Basic.VirtualMachine.Exceptions;

public class ScanException : Exception
{
    public ScanException(string message) : base(message)
    {
    }
}