using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShadUI;
using TestJob_Avalonia.Models;
using TestJob_Avalonia.Services.Navigations;

namespace TestJob_Avalonia.ViewModels;

/// <summary>
/// ViewModel главного окна
/// </summary>
public partial class MainWindowViewModel : ViewModelBase
{
    #region Dependencies
    private readonly INavigationService _navigationService;
    private readonly ThemeWatcher _themeWatcher;
    #endregion

    #region Properties
    /// <summary>
    /// Пункты навигационного меню, отображаемые над основной областью
    /// </summary>
    public IReadOnlyList<NavigationItem> NavigationItems { get; }

    /// <summary>
    /// ViewModel активного раздела, отображаемого в основной области
    /// </summary>
    [ObservableProperty]
    public partial ViewModelBase? CurrentPage { get; set; }

    /// <summary>
    /// Блок авторизации, отображается постоянно и не участвует в навигации между разделами
    /// </summary>
    public AuthViewModel AuthViewModel { get; }

    /// <summary>
    /// Менеджер диалогов для создания и управления диалоговыми окнами
    /// </summary>
    public DialogManager DialogManager { get; }

    /// <summary>
    /// Менеджер уведомлений для отображения всплывающих сообщений
    /// </summary>
    public ToastManager ToastManager { get; }

    /// <summary>
    /// Текущая тема приложения
    /// </summary>
    [ObservableProperty]
    public partial ThemeMode CurrentTheme { get; set; }
    #endregion

    public MainWindowViewModel(INavigationService navigationService, ThemeWatcher themeWatcher, DialogManager dialogManager,
        ToastManager toastManager, AuthViewModel authViewModel)
    {
        _navigationService = navigationService;
        _themeWatcher = themeWatcher;
        DialogManager = dialogManager;
        ToastManager = toastManager;
        AuthViewModel = authViewModel;

        NavigationItems =
        [
            new("\ue57d", "Личная информация", RedirectionUserInfoCommand),
            new("\ue36e", "Информация о разработчике", RedirectionDevInfoCommand)
        ];

        _navigationService.PageChanged += vm => CurrentPage = vm;
    }

    /// <summary>
    /// Открывает раздел по умолчанию
    /// </summary>
    public override Task InitializeAsync() => _navigationService.RequestNavigation<UserInfoViewModel>();

    #region Methods
    /// <summary>
    /// Переход на страницу - Информация о пользователе
    /// </summary>
    [RelayCommand]
    private Task RedirectionUserInfo() => _navigationService.RequestNavigation<UserInfoViewModel>();

    /// <summary>
    /// Переход на страницу - Информация о разработчике
    /// </summary>
    [RelayCommand]
    private Task RedirectionDevInfo() => _navigationService.RequestNavigation<DevInfoViewModel>();

    /// <summary>
    /// Переключает тему приложения по кругу
    /// </summary>
    [RelayCommand]
    private void SwitchTheme()
    {
        CurrentTheme = CurrentTheme switch
        {
            ThemeMode.System => ThemeMode.Light,
            ThemeMode.Light => ThemeMode.Dark,
            _ => ThemeMode.System
        };

        _themeWatcher.SwitchTheme(CurrentTheme);
    }
    #endregion
}