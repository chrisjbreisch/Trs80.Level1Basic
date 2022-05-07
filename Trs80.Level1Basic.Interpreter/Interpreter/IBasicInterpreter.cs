using Trs80.Level1Basic.Interpreter.Parser;
using Trs80.Level1Basic.Interpreter.Parser.Expressions;
using Trs80.Level1Basic.Interpreter.Parser.Statements;

namespace Trs80.Level1Basic.Interpreter.Interpreter;

public interface IBasicInterpreter : IExpressionVisitor, IStatementVisitor
{
    int CursorX { get; }
    int CursorY { get; }
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