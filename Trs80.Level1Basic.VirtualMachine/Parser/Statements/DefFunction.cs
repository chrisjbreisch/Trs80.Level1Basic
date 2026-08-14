using Trs80.Level1Basic.VirtualMachine.Exceptions;
using Trs80.Level1Basic.VirtualMachine.Parser.Expressions;
using Trs80.Level1Basic.VirtualMachine.Scanner;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

public class DefFunction : Statement
{
    public string Name { get; }
    public string Parameter { get; }
    public Expression Body { get; }

    public DefFunction(string name, string parameter, Expression body)
    {
        Name = name;
        Parameter = parameter;
        Body = body;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        CheckExceptions();
        return visitor.VisitDefFunctionStatement(this);
    }
}
