using System;
using System.Threading.Tasks;
using TestJob_Avalonia.ViewModels;

namespace TestJob_Avalonia.Services.Navigations;

/// <summary>
/// Интерфейс для навигации между страницами
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Action для уведомления подписчиков о смене страницы
    /// </summary>
    event Action<ViewModelBase>? PageChanged;

    /// <summary>
    /// Метод для смены текущего представления
    /// </summary>
    Task RequestNavigation<T>() where T : ViewModelBase;

    /// <summary>
    /// Позволяет сменить страницу
    /// </summary>
    /// <param name="viewModelType">Тип страницы, которую необходимо отобразить</param>
    Task NavigateTo(Type viewModelType);

    /// <summary>
    /// Позволяет сменить страницу
    /// </summary>
    /// <param name="viewModel">Cтраница, которая реализует ViewModelBase</param>
    Task NavigateTo(ViewModelBase viewModel);

    /// <summary>
    /// Возвращает ViewModel с учётом времени жизни. Transient создаётся через <see cref="ActivatorUtilities"/>
    /// зависимости берутся из контейнера, но сам экземпляр контейнер не запоминает и не удерживает после освобождения.
    /// Singleton берётся из контейнера
    /// </summary>
    /// <param name="viewModelType">Тип ViewModel</param>
    ViewModelBase GetViewModel(Type viewModelType);
}