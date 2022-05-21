using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

public class LineList : Statement, ICollection<IStatement>
{
    private readonly List<IStatement> _statements = new();

    public void Replace(int lineNumber, IStatement item)
    {
        IStatement original = _statements.FirstOrDefault(s => s.LineNumber == lineNumber);

        if (original == null)
        {
            Add(item);
            return;
        }

        if (original.Previous != null)
        {
            original.Previous.Next = item;
            item.Previous = original.Previous;
        }

        if (original.Next != null)
        {
            item.Next = original.Next;
            original.Next.Previous = item;
        }

        int index = _statements.IndexOf(original);

        if (index >= 0)
        {
            _statements.RemoveAt(index);
            _statements.Insert(index, item);
        }
        else
            _statements.Add(item);
    }

    public void Add(IStatement item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        int lineNumber = item.LineNumber;
        IStatement original = _statements.FirstOrDefault(s => s.LineNumber == lineNumber);

        if (original != null)
            throw new InvalidOperationException("An item with the same line number has already been added.");
        
        if (_statements.Count == 0)
            _statements.Add(item);
        else
        {
            IStatement predecessor = _statements.LastOrDefault(s => s.LineNumber < lineNumber);
            if (predecessor != null)
            {
                predecessor.Next = item;
                int index = _statements.IndexOf(predecessor);
                _statements.Insert(Math.Min(index + 1, _statements.Count), item);
                item.Previous = predecessor;
            }

            IStatement successor = _statements.FirstOrDefault(s => s.LineNumber > lineNumber);
            if (successor == null) return;

            item.Next = successor;
            if (predecessor == null)
            {
                int index = _statements.IndexOf(successor);
                _statements.Insert(index, item);
            }

            successor.Previous = item;
        }
    }

    public void Clear()
    {
        _statements.Clear();
    }

    public bool Contains(IStatement item)
    {
        if (item == null) return false;

        return _statements.Contains(item);
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
        IStatement original = _statements.FirstOrDefault(s => s.LineNumber == item.LineNumber && s.SourceLine == item.SourceLine);

        if (original == null) return false;

        if (original.Previous != null)
            original.Previous.Next = original.Next;

        if (original.Next != null)
            original.Next.Previous = original.Previous;

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