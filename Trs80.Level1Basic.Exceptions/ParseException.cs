using System;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Exceptions
{
    public class ParseException : Exception
    {
        public int LineNumber { get; }
        public string Statement { get; }
        public ParseException(int lineNumber, string statement, string message) : base(message)
        {
            LineNumber = lineNumber;
            Statement = statement;
        }

    }
}
