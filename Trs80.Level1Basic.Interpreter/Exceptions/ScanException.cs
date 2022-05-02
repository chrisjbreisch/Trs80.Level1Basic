using System;

namespace Trs80.Level1Basic.Interpreter.Exceptions;

public class ScanException : Exception
{
    public ScanException(string message) : base(message)
    {
    }
}