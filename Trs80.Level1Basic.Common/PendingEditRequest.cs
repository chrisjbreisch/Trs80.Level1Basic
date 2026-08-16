namespace Trs80.Level1Basic.Common;

public sealed class PendingEditRequest
{
    private int? _lineNumber;

    public void Request(int lineNumber) => _lineNumber = lineNumber;

    public bool TryTake(out int lineNumber)
    {
        lineNumber = _lineNumber ?? 0;
        bool hasRequest = _lineNumber.HasValue;
        _lineNumber = null;
        return hasRequest;
    }
}
