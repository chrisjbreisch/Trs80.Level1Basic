using Trs80.Level1Basic.Services.Parser.Expressions;
using Trs80.Level1Basic.Services.Parser.Statements;

namespace Trs80.Level1Basic.Services.Interpreter;

public interface IBasicInterpreter : IExpressionVisitor, IStatementVisitor
{
    void Interpret(string source);
    void Execute(Statement statement);

    BasicFunctionImplementations Functions { get; }
    void WriteToPosition(int position);
    string PadQuadrant();
    void Set(int x, int y);
    void Reset(int x, int y);
    bool Point(int x, int y);
}