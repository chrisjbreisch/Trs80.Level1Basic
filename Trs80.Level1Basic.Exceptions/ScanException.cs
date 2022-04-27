using System;

namespace Trs80.Level1Basic.Exceptions;

public class ScanException : Exception
{
    public ScanException(string message) : base(message)
    {
    }
}