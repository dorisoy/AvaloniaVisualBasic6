using System;
using System.Collections.Generic;
using Avalonia.Media;
using AvaloniaVisualBasic.IDE;

namespace AvaloniaVisualBasic.Forms.ViewModels;

public abstract class BaseEditorWindowViewModel : IMdiWindow, IDisposable
{
    private List<IDisposable>? disposables;
    public abstract string Title { get; }
    public abstract IImage Icon { get; }
    public event Action<IMdiWindow>? CloseRequest;

    protected T AutoDispose<T>(T t) where T : IDisposable
    {
        disposables ??= new();
        disposables.Add(t);
        return t;
    }

    public virtual void Dispose()
    {
        if (disposables == null)
            return;

        for (int i = disposables.Count - 1; i >= 0; --i)
            disposables[i].Dispose();

        disposables.Clear();
    }

    protected void RequestClose() => CloseRequest?.Invoke(this);
}