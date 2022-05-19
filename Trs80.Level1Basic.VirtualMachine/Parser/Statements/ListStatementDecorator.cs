using System;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

public interface IListStatementDecorator : IStatement
{
}

public class ListStatementDecorator : IListStatementDecorator
{
    private readonly IStatement _statement;

    public ListStatementDecorator(IStatement statement)
    {
        _statement = statement ?? throw new ArgumentNullException(nameof(statement));
    }

    public int LineNumber
    {
        get => _statement.LineNumber;
        set => _statement.LineNumber = value;
    }

    public string SourceLine
    {
        get => _statement.SourceLine;
        set => _statement.SourceLine = value;
    }

    public IStatement Next
    {
        get => _statement.Next;
        set => _statement.Next = value;
    }

    public IStatement Previous
    {
        get => _statement.Previous;
        set => _statement.Previous = value;
    }

    public IStatement Parent
    {
        get => _statement.Parent;
        set => _statement.Parent = value;
    }

    public T Accept<T>(IVisitor<T> visitor)
    {
        throw new NotImplementedException();
    }
}
