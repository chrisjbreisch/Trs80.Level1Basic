using System;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

public interface IListItemDecorator : IStatement
{
    public Type BaseType();
    public IStatement UnDecorate();
}

public class ListItemDecorator : IListItemDecorator
{
    private readonly IStatement _statement;

    public ListItemDecorator(IStatement statement)
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
        return _statement.Accept(visitor);
    }

    public IStatement UnDecorate()
    {
        return _statement;
    }

    public Type BaseType()
    {
        return _statement.GetType();
    }
}
