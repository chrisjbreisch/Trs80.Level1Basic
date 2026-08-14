using System.Collections.Generic;

using Trs80.Level1Basic.VirtualMachine.Exceptions;
using Trs80.Level1Basic.VirtualMachine.Scanner;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

public class DefType : Statement
{
    public TokenType Type { get; }
    public List<string> Names { get; }

    public DefType(TokenType type, List<string> names)
    {
        Type = type;
        Names = names ?? new List<string>();
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        CheckExceptions();
        return visitor.VisitDefTypeStatement(this);
    }
}
