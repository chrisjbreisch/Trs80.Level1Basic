using System;
using System.Collections.Generic;

namespace Trs80.Level1Basic.Common;

public sealed class LineEditorHistory
{
    private readonly List<string> _entries = new();
    private int _recallIndex;
    private string _draft = string.Empty;

    public LineEditorHistory()
    {
        ResetRecall();
    }

    public void Add(string line)
    {
        if (string.IsNullOrEmpty(line))
            return;

        _entries.Add(line);
        ResetRecall();
    }

    public bool TryGetPrevious(out string line)
    {
        return TryGetPrevious(string.Empty, out line);
    }

    public bool TryGetPrevious(string currentLine, out string line)
    {
        if (_entries.Count == 0)
        {
            line = string.Empty;
            return false;
        }

        if (_recallIndex == _entries.Count)
            _draft = currentLine;

        _recallIndex = Math.Max(0, _recallIndex - 1);
        line = _entries[_recallIndex];
        return true;
    }

    public bool TryGetNext(out string line)
    {
        if (_entries.Count == 0)
        {
            line = string.Empty;
            return false;
        }

        if (_recallIndex >= _entries.Count)
        {
            line = _draft;
            return true;
        }

        _recallIndex++;
        line = _recallIndex < _entries.Count ? _entries[_recallIndex] : _draft;
        return true;
    }

    private void ResetRecall()
    {
        _recallIndex = _entries.Count;
        _draft = string.Empty;
    }
}