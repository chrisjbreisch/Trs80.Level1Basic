using System;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Exceptions
{
    public class ParseException : Exception
    {
        public Token Token { get; }
        public ParseException(Token token, string message) : base(message)
        {
            Token = token;
        }

    }
}
