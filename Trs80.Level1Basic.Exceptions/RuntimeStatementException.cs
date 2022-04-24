using System;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Exceptions
{
    public class RuntimeStatementException : Exception
    {
        public int LineNumber { get;  }

        public RuntimeStatementException(int lineNumber, string message) : base(message)
        {
            LineNumber = lineNumber;
        }
    }
}
