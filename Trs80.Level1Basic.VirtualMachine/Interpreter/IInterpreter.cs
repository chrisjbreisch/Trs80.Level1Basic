using Trs80.Level1Basic.Common;
using Trs80.Level1Basic.VirtualMachine.Parser.Statements;

namespace Trs80.Level1Basic.VirtualMachine.Interpreter;

public interface IInterpreter : Parser.Expressions.IVisitor<object>, IVisitor<Void>
{
    void Interpret(Statement statement);
    void Execute(IStatement statement);
}