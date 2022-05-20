using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

public class LineList : Statement, ICollection<IStatement>
{
    private readonly List<IListItemDecorator> _statements = new();

    private IListItemDecorator Decorate(IStatement statement)
    {
        return statement is IListItemDecorator decorated ? decorated : new ListItemDecorator(statement);
    }

    public void Replace(int lineNumber, IStatement item)
    {
        IListItemDecorator original = _statements.FirstOrDefault(s => s.LineNumber == lineNumber);

        if (original == null)
        {
            Add(item);
            return;
        }

        IListItemDecorator decorated = Decorate(item);

        if (original.Previous is IListItemDecorator predecessor)
        {
            predecessor.Next = decorated;
            decorated.Previous = predecessor;
        }

        if (original.Next is IListItemDecorator successor)
        {
            decorated.Next = successor;
            successor.Previous = decorated;
        }

        decorated.Parent = this;
        int index = _statements.IndexOf(original);

        if (index >= 0)
        {
            _statements.RemoveAt(index);
            _statements.Insert(index, decorated);
        }
        else
            _statements.Add(decorated);
    }

    public void Add(IStatement item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        int lineNumber = item.LineNumber;
        IListItemDecorator original = _statements.FirstOrDefault(s => s.LineNumber == lineNumber);

        if (original != null)
            throw new InvalidOperationException("An item with the same line number has already been added.");

        IListItemDecorator decorated = Decorate(item);

        if (_statements.Count == 0)
            _statements.Add(decorated);
        else
        {
            IListItemDecorator predecessor = _statements.LastOrDefault(s => s.LineNumber < lineNumber);
            if (predecessor != null)
            {
                predecessor.Next = decorated;
                int index = _statements.IndexOf(predecessor);
                _statements.Insert(Math.Min(index + 1, _statements.Count), decorated);
                decorated.Previous = predecessor;
            }

            IListItemDecorator successor = _statements.FirstOrDefault(s => s.LineNumber > lineNumber);
            if (successor == null) return;

            decorated.Next = successor;
            if (predecessor == null)
            {
                int index = _statements.IndexOf(successor);
                _statements.Insert(index, decorated);
            }

            successor.Previous = decorated;
        }
    }

    public void Clear()
    {
        _statements.Clear();
    }

    public bool Contains(IStatement item)
    {
        if (item == null) return false;

        return item is IListItemDecorator ?
            _statements.Contains(item) :
            _statements.Any(s => s.LineNumber == item.LineNumber && s.SourceLine == item.SourceLine);
    }

    public bool ContainsLine(int lineNumber)
    {
        return _statements.Count > 0 && lineNumber >= 0 && _statements.Any(s => s.LineNumber == lineNumber);
    }

    public void CopyTo(IStatement[] array, int arrayIndex)
    {
        for (int i = 0; i < array.Length; i++)
            array[arrayIndex + i] = this[i];
    }

    public bool Remove(IStatement item)
    {
        if (item == null) return false;
        IListItemDecorator original = _statements.FirstOrDefault(s => s.LineNumber == item.LineNumber && s.SourceLine == item.SourceLine);

        if (original == null) return false;

        if (original.Previous is IListItemDecorator predecessor)
            predecessor.Next = original.Next;

        if (original.Next is IListItemDecorator successor)
            successor.Previous = original.Previous;

        _statements.Remove(original);
        return true;
    }

    public int Count => _statements.Count;
    public bool IsReadOnly => false;

    public IStatement this[int index] => _statements[index];

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