using System;

namespace ILAccess.Tests.AssemblyToProcess;

public class Disposable : IDisposable
{
    private readonly Action _disposeBody;

    public Disposable(Action disposeBody)
    {
        _disposeBody = disposeBody ?? throw new ArgumentNullException(nameof(disposeBody));
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _disposeBody();
    }

    public static IDisposable Empty => Create(() => { });
    public static IDisposable Create(Action action) => new Disposable(action);
}