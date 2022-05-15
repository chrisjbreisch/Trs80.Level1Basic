using Trs80.Level1Basic.Common;
using Trs80.Level1Basic.VirtualMachine.Parser;
using Trs80.Level1Basic.VirtualMachine.Parser.Statements;

namespace Trs80.Level1Basic.VirtualMachine.Interpreter;


public interface IInterpreter : Parser.Expressions.IVisitor<object>, IVisitor<Void>
{
    Statement CurrentStatement { get; }
    void Interpret(ParsedLine line);
    void Execute(Statement statement);
    FunctionImplementations Functions { get; }
    string PadToPosition(int position);
    string PadToQuadrant();
    void Set(int x, int y);
    void Reset(int x, int y);
    bool Point(int x, int y);
    int MemoryInUse();
}