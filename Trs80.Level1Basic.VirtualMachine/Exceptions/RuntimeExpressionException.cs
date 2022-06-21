using System;
using Trs80.Level1Basic.VirtualMachine.Scanner;

namespace Trs80.Level1Basic.VirtualMachine.Exceptions;

public class RuntimeExpressionException : Exception
{
    public Token Token { get; }

    public RuntimeExpressionException(Token token, string message) : base(message)
    {
        Token = token;
    }
}