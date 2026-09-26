using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TestJob_Avalonia.Services.Extensions;
using TestJob_Avalonia.ViewModels;

namespace TestJob_Avalonia.Services.Navigations;

/// <summary>
/// Сервис навигации между страницами приложения
/// </summary>
public partial class NavigationService(IServiceProvider serviceProvider) : INavigationService
{
    /// <inheritdoc/>
    public event Action<ViewModelBase>? PageChanged;

    /// <summary>
    /// Содержит актуальную VM
    /// </summary>
    private ViewModelBase? _currentViewModel;

    #region Методы навигации
    /// <inheritdoc/>
    public async Task RequestNavigation<T>() where T : ViewModelBase => await NavigateTo(typeof(T));

    /// <inheritdoc/>
    public ViewModelBase GetViewModel(Type viewModelType) => (ViewModelBase)serviceProvider.GetRequiredService(viewModelType);

    /// <inheritdoc/>
    public async Task NavigateTo(Type viewModelType)
    {
        if (_currentViewModel != null)
        {
            if (ViewModelLifetimeRegistry.IsTransient(_currentViewModel.GetType()))
                if (_currentViewModel is IDisposable disposable)
                    disposable.Dispose();
        }

        var vm = GetViewModel(viewModelType);
        await vm.Initialize();

        _currentViewModel = vm;
        PageChanged?.Invoke(vm);
    }

    /// <inheritdoc/>
    public async Task NavigateTo(ViewModelBase viewModel)
    {
        Type viewModelType = viewModel.GetType();

        if (_currentViewModel != null)
        {
            if (ViewModelLifetimeRegistry.IsTransient(_currentViewModel.GetType()))
                if (_currentViewModel is IDisposable disposable)
                    disposable.Dispose();
        }

        await viewModel.Initialize();

        _currentViewModel = viewModel;
        PageChanged?.Invoke(viewModel);
    }
    #endregion
}