using System.Collections.Generic;
using System.Linq;

namespace Trs80.Level1Basic.Common;

public sealed class LineEditorBuffer
{
    private readonly List<char> _characters;

    public LineEditorBuffer(string text = "")
    {
        _characters = text.ToList();
    }

    public int CursorIndex { get; private set; }

    public int Length => _characters.Count;

    public override string ToString() => new(_characters.ToArray());

    public void MoveLeft()
    {
        if (CursorIndex > 0)
            CursorIndex--;
    }

    public void MoveRight()
    {
        if (CursorIndex < _characters.Count)
            CursorIndex++;
    }

    public void MoveHome()
    {
        CursorIndex = 0;
    }

    public void MoveEnd()
    {
        CursorIndex = _characters.Count;
    }

    public void Insert(char character)
    {
        _characters.Insert(CursorIndex, character);
        CursorIndex++;
    }

    public void Replace(string text)
    {
        _characters.Clear();
        _characters.AddRange(text);
        CursorIndex = _characters.Count;
    }

    public void Clear()
    {
        _characters.Clear();
        CursorIndex = 0;
    }

    public void Backspace()
    {
        if (CursorIndex <= 0)
            return;

        _characters.RemoveAt(--CursorIndex);
    }

    public void Delete()
    {
        if (CursorIndex < _characters.Count)
            _characters.RemoveAt(CursorIndex);
    }
}