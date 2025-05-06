namespace ClipboardTranslator.Core;

public abstract class DisposableBase : IDisposable
{
    private bool _disposed;

    protected virtual void DisposeManaged() { }

    protected virtual void DisposeUnmanaged() { }

    protected void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(GetType().Name);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            DisposeManaged();
        }

        DisposeUnmanaged();

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~DisposableBase()
    {
        Dispose(false);
    }
}

