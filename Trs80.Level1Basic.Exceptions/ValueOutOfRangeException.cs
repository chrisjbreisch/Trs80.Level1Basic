using System;

namespace Trs80.Level1Basic.Exceptions
{
    public class ValueOutOfRangeException : Exception
    {
        public int LineNumber { get; }
        public ValueOutOfRangeException(int lineNumber, string message) : base(message)
        {
            LineNumber = lineNumber;
        }
    }
}
