using System;

using Trs80.Level1Basic.Interpreter.Scanner;

namespace Trs80.Level1Basic.Interpreter.Exceptions;

public class RuntimeExpressionException : Exception
{
    public Token Token { get; }

    public RuntimeExpressionException(Token token, string message) : base(message)
    {
        Token = token;
    }
}