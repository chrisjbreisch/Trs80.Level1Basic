using System;
using System.Collections.Generic;

using Trs80.Level1Basic.VirtualMachine.Exceptions;

namespace Trs80.Level1Basic.VirtualMachine.Interpreter;

public class DataElements
{
    private readonly List<(dynamic Value, int LineNumber)> _dataElements = new();
    private int _listIndex;

    public void Add(dynamic value, int lineNumber)
    {
        _dataElements.Add((value, lineNumber));
    }

    public void MoveFirst()
    {
        _listIndex = 0;
    }

    public void MoveToLine(int lineNumber)
    {
        _listIndex = _dataElements.FindIndex(element => element.LineNumber >= lineNumber);
        if (_listIndex < 0)
            _listIndex = _dataElements.Count;
    }

    public dynamic GetNext()
    {
        return _listIndex < _dataElements.Count
            ? _dataElements[_listIndex++].Value
            : throw new OutOfDataException(-1, string.Empty, 0, "No more DATA elements are available.");
    }

    public void Clear()
    {
        _dataElements.Clear();
    }
}