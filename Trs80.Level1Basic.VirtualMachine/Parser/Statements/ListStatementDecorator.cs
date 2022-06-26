using System;
using Trs80.Level1Basic.VirtualMachine.Exceptions;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

public class ListStatementDecorator : IListStatementDecorator
{
    private readonly IStatement _statement;

    public IStatement Enclosing { get; set; }


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

    public ParseException ParseException { get; set; }

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
