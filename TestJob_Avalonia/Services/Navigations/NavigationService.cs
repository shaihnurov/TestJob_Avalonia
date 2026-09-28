using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TestJob_Avalonia.Services.Extensions;
using TestJob_Avalonia.ViewModels;

namespace TestJob_Avalonia.Services.Navigations;

/// <summary>
/// Сервис навигации между разделами приложения.
/// Transient VM создаются в обход отслеживания контейнером и освобождаются после ухода со страницы,
/// singleton берутся из контейнера и сохраняют состояние
/// </summary>
public sealed class NavigationService(IServiceProvider serviceProvider) : INavigationService
{
    private readonly SemaphoreSlim _navigationLock = new(1, 1);
    private ViewModelBase? _currentViewModel;

    /// <inheritdoc/>
    public event Action<ViewModelBase>? PageChanged;

    /// <inheritdoc/>
    public Task RequestNavigation<T>() where T : ViewModelBase => NavigateTo(typeof(T));

    /// <inheritdoc/>
    public Task NavigateTo(Type viewModelType) => NavigateCore(() => GetViewModel(viewModelType));

    /// <inheritdoc/>
    public Task NavigateTo(ViewModelBase viewModel) => NavigateCore(() => viewModel);

    /// <inheritdoc/>
    public ViewModelBase GetViewModel(Type viewModelType) => ViewModelLifetimeRegistry.IsTransient(viewModelType) 
        ? (ViewModelBase)ActivatorUtilities.CreateInstance(serviceProvider, viewModelType) : (ViewModelBase)serviceProvider.GetRequiredService(viewModelType);

    /// <summary>
    /// Общая логика перехода, создание и инициализация новой ViewModel, переключение экрана, освобождение предыдущей
    /// </summary>
    /// <param name="resolve">Фабрика новой ViewModel вызывается внутри блокировки</param>
    private async Task NavigateCore(Func<ViewModelBase> resolve)
    {
        await _navigationLock.WaitAsync();
        try
        {
            var previous = _currentViewModel;
            var next = resolve();

            try
            {
                await next.InitializeAsync();
            }
            catch
            {
                DisposeIfTransient(next);
                throw;
            }

            _currentViewModel = next;
            PageChanged?.Invoke(next);

            if (!ReferenceEquals(previous, next))
                DisposeIfTransient(previous);
        }
        finally
        {
            _navigationLock.Release();
        }
    }

    /// <summary>
    /// Освобождает ViewModel, если она Transient. Singleton не трогаем: они переиспользуются.
    /// </summary>
    private static void DisposeIfTransient(ViewModelBase? viewModel)
    {
        if (viewModel is not null && ViewModelLifetimeRegistry.IsTransient(viewModel.GetType()))
            viewModel.Dispose();
    }
}