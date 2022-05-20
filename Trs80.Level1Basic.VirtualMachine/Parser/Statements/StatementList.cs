using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

public class StatementList : Statement, IList<IStatement>
{
    private readonly List<IListStatementDecorator> _statements = new();

    private IListStatementDecorator Decorate(IStatement statement)
    {
        return new ListStatementDecorator(statement);
    }

    public StatementList Insert(IStatement item)
    {
        IListStatementDecorator original = item is IListStatementDecorator d ? d : _statements.FirstOrDefault(s => s.LineNumber == item.LineNumber);

        if (original != null)
            return Replace(item);

        IListStatementDecorator decorated = Decorate(item);

        IListStatementDecorator predecessor = _statements.LastOrDefault(s => s.LineNumber < item.LineNumber);
        if (predecessor != null)
        {
            predecessor.Next = decorated;
            int index = _statements.IndexOf(predecessor);
            _statements.Insert(Math.Min(index + 1, _statements.Count), decorated);
        }

        IListStatementDecorator successor = _statements.FirstOrDefault(s => s.LineNumber > item.LineNumber);

        if (successor != null)
        {
            decorated.Next = successor;
            if (predecessor == null)
            {
                int index = _statements.IndexOf(successor);
                _statements.Insert(index, decorated);
            }
        }

        decorated.Parent = this;
        return this;
    }

    private StatementList Replace(IStatement item)
    {
        IListStatementDecorator original = item is IListStatementDecorator o ? o : _statements.FirstOrDefault(s => s.LineNumber == item.LineNumber);

        if (original == null)
            return Insert(item);

        IListStatementDecorator decorated = item is IListStatementDecorator d ? d : Decorate(item);

        if (original.Previous is IListStatementDecorator predecessor)
        {
            predecessor.Next = decorated;
            decorated.Previous = predecessor;
        }

        if (original.Next is IListStatementDecorator successor)
        {
            decorated.Next = successor;
            successor.Previous = decorated;
        }

        decorated.Parent = this;
        int index = _statements.IndexOf(original);

        _statements.Remove(original);

        _statements.Insert(index, decorated);


        return this;
    }

    public StatementList AddOrReplace(IStatement item)
    {
        IListStatementDecorator original = item is IListStatementDecorator d ? d : _statements.FirstOrDefault(s => s.LineNumber == item.LineNumber);

        if (original == null)
            Insert(item);
        else
            Replace(item);

        return this;
    }

    public void Add(IStatement item)
    {
        IListStatementDecorator decorated = item is IListStatementDecorator d ? d : Decorate(item);
        
        if (_statements.Count > 0)
        {
            IListStatementDecorator last = _statements[^1];
            last.Next = decorated;
            decorated.Previous = last;
        }

        decorated.Parent = this;
        _statements.Add(decorated);
    }

    public void Clear()
    {
        _statements.Clear();
    }

    public bool Contains(IStatement item)
    {
        return _statements.Contains(Decorate(item));
    }

    public void CopyTo(IStatement[] array, int arrayIndex)
    {
        for (int i = 0; i < array.Length; i++)
            array[arrayIndex + i] = this[i];
    }

    public bool Remove(IStatement item)
    {
        if (item == null) return false;
        IListStatementDecorator original = item is IListStatementDecorator d ? d : _statements.FirstOrDefault(s => s.LineNumber == item.LineNumber);

        if (original == null) return false;

        if (original.Previous is IListStatementDecorator predecessor)
            predecessor.Next = original.Next;

        if (original.Next is IListStatementDecorator successor)
            successor.Previous = original.Previous;

        _statements.Remove(original);

        original.Next = null;
        original.Previous = null;
        original.Parent = null;

        return true;
    }

    public int Count => _statements.Count;
    public bool IsReadOnly => false;

    public int IndexOf(IStatement item)
    {
        if (item is IListStatementDecorator decorated)
            return _statements.IndexOf(decorated);

        return _statements.IndexOf(Decorate(item));
    }

    public void Insert(int index, IStatement item)
    {
        if (index > 0)
        {
            IListStatementDecorator predecessor = _statements[index - 1];
            item!.Previous = predecessor;
            predecessor.Next = item;
        }

        if (index <= _statements.Count)
        {
            IListStatementDecorator successor = _statements[index];
            item!.Next = successor;
            successor.Previous = item;
        }
        IListStatementDecorator decorated = item is IListStatementDecorator d ? d : Decorate(item);
        decorated.Parent = this;
        _statements.Insert(index, decorated);
    }

    public void RemoveAt(int index)
    {
        if (index > 0)
        {
            IListStatementDecorator predecessor = _statements[index - 1];
            predecessor.Next = _statements[index].Next;
        }

        if (index < _statements.Count)
        {
            IListStatementDecorator successor = _statements[index + 1];
            successor.Previous = _statements[index].Previous;
        }
        _statements[index].Previous = null;
        _statements[index].Next = null;
        _statements[index].Parent = null;
        _statements.RemoveAt(index);
    }

    public IStatement this[int index]
    {
        get { return _statements[index]; }
        set { AddOrReplace(value); }
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return default;
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