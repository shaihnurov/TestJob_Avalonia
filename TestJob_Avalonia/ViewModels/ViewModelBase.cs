using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TestJob_Avalonia.ViewModels;

/// <summary>
/// Базовый класс для всех ViewModel приложения
/// </summary>
public abstract class ViewModelBase : ObservableValidator, IDisposable
{
    private bool _disposed;

    /// <summary>
    /// Признак того, что ViewModel уже освобождена. Позволяет асинхронному коду проверить,
    /// что пользователь не ушёл со страницы, пока выполнялся запрос, и не обновлять ViewModel
    /// </summary>
    protected bool IsDisposed => _disposed;

    /// <summary>
    /// Вызывается при навигации на эту ViewModel, до её отображения. Здесь можно подгрузить данные
    /// </summary>
    public virtual Task InitializeAsync() => Task.CompletedTask;

    /// <summary>
    /// Освобождает ресурсы ViewModel. Повторные вызовы игнорируются <see cref="Dispose(bool)"/> выполняется ровно один раз
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Точка расширения для наследников, можно отписываться от событий, главное не забыть вызвать base.Dispose(disposing)
    /// </summary>
    /// <param name="disposing">true, если вызвано из <see cref="Dispose()"/> ресурсы можно освобождать</param>
    protected virtual void Dispose(bool disposing)
    {
    }
}