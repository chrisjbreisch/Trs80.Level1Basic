﻿using System;

namespace Trs80.Level1Basic.Exceptions
{
    public class ScanException : Exception
    {
        public int LineNumber { get; }
        public ScanException(int lineNumber, string message) : base(message)
        {
            LineNumber = lineNumber;
        }
    }
}
