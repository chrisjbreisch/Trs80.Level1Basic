using Trs80.Level1Basic.VirtualMachine.Parser;
using Trs80.Level1Basic.VirtualMachine.Parser.Expressions;
using Trs80.Level1Basic.VirtualMachine.Parser.Statements;

namespace Trs80.Level1Basic.VirtualMachine.Interpreter;

public interface IInterpreter : IExpressionVisitor<object>, IStatementVisitor<object>
{
    Statement CurrentStatement { get; }
    void Interpret(ParsedLine line);
    void Execute(Statement statement);
    BasicFunctionImplementations Functions { get; }
    void WriteToPosition(int position);
    string PadQuadrant();
    void Set(int x, int y);
    void Reset(int x, int y);
    bool Point(int x, int y);
    int MemoryInUse();
}