//
//
// This file was automatically generated by generateAst
// at 2022-06-20 11:38:10 AM UTC. Do not modify.
//
//

using Trs80.Level1Basic.VirtualMachine.Parser.Expressions;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

public class Save : Statement
{
    public Expression Path { get; init; }

    public Save(Expression path)
    {
        Path = path;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitSaveStatement(this);
    }
}
