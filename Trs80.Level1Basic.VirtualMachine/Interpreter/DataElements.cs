using System;
using System.Collections.Generic;

using Trs80.Level1Basic.VirtualMachine.Exceptions;

namespace Trs80.Level1Basic.VirtualMachine.Interpreter;

public class DataElements
{
    private readonly List<dynamic> _dataElements = new();
    private int _listIndex;

    public void Add(dynamic value)
    {
        _dataElements.Add(value);
    }

    public void MoveFirst()
    {
        _listIndex = 0;
    }

    public dynamic GetNext()
    {
        return _listIndex < _dataElements.Count
            ? _dataElements[_listIndex++]
            : throw new OutOfDataException(-1, string.Empty, 0, "No more DATA elements are available.");
    }

    public void Clear()
    {
        _dataElements.Clear();
    }
}