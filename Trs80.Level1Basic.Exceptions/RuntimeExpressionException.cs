using System;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Exceptions
{
    public class RuntimeExpressionException : Exception
    {
        public Token Token { get;  }

        public RuntimeExpressionException(Token token, string message) : base(message)
        {
            Token = token;
        }
    }
}
