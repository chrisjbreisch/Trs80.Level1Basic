using System;

namespace Trs80.Level1Basic.Common;

public class DisposableBase : IDisposable
{
    private bool _isDisposed;

    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed) return;

        _isDisposed = true;

        if (disposing)
            DisposeExplicit();

        DisposeImplicit();

        GC.SuppressFinalize(this);
    }

    protected virtual void DisposeExplicit() { }
    protected virtual void DisposeImplicit() { }

    ~DisposableBase()
    {
        Dispose(false);
    }

    public virtual void Dispose()
    {
        Dispose(true);
    }
}