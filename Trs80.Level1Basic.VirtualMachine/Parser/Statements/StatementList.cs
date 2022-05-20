using System;
using System.Collections;
using System.Collections.Generic;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

public class StatementList : Statement, ICollection<IStatement>
{
    private readonly List<IListItemDecorator> _statements = new();

    public StatementList()
    {
    }
    private IListItemDecorator Decorate(IStatement statement)
    {
        return statement is IListItemDecorator decorated ? decorated : new ListItemDecorator(statement);
    }

    public void Add(IStatement item)
    {
        IListItemDecorator decorated = Decorate(item);

        if (_statements.Count > 0)
        {
            IListItemDecorator predecessor = _statements[^1];
            predecessor.Next = decorated;
            decorated.Previous = predecessor;
        }
        _statements.Add(decorated);

        decorated.Parent = Parent;
    }

    public void Clear()
    {
        _statements.Clear();
    }

    public bool Contains(IStatement item)
    {
        throw new InvalidOperationException();
    }

    public void CopyTo(IStatement[] array, int arrayIndex)
    {
        for (int i = 0; i < array.Length; i++)
            array[arrayIndex + i] = this[i];
    }

    public bool Remove(IStatement item)
    {
        throw new InvalidOperationException();
    }

    public int Count => _statements.Count;
    public bool IsReadOnly => false;

    public IStatement this[int index] => _statements[index];

    public override T Accept<T>(IVisitor<T> visitor)
    {
        T result = default;

        foreach (IListItemDecorator statement in _statements)
        {
            result = statement.Accept(visitor);
            if (statement.BaseType() == typeof(Goto))
                break;
        }

        return result;
    }

    public IEnumerator<IStatement> GetEnumerator()
    {
        return _statements.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _statements.GetEnumerator();
    }
}