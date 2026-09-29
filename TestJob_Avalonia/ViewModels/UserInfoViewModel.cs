using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using ShadUI;
using TestJob_Avalonia.Models.Dto;
using TestJob_Avalonia.Services.Auth;
using TestJob_Avalonia.Services.Auth.API;

namespace TestJob_Avalonia.ViewModels;

/// <summary>
/// ViewModel раздела Личная информация (View_1)
/// </summary>
public sealed partial class UserInfoViewModel : ViewModelBase
{
    #region Dependencies
    private readonly IAuthApiService _authApiService;
    private readonly IAuthStateService _authStateService;
    private readonly ToastManager _toastManager;
    #endregion

    /// <summary>
    /// Источник отмены текущей загрузки. Нужен, чтобы устаревший запрос не перезаписал более новый
    /// </summary>
    private CancellationTokenSource? _loadCts;

    #region Properties
    /// <summary>
    /// Данные пользователя для отображения
    /// </summary>
    [ObservableProperty]
    public partial UserDto? User { get; set; }

    /// <summary>
    /// Признак того, что пользователь авторизован. Определяет, что показывать на странице
    /// </summary>
    public bool IsAuthenticated => _authStateService.IsAuthenticated;
    #endregion

    public UserInfoViewModel(IAuthApiService authApiService, IAuthStateService authStateService, ToastManager toastManager)
    {
        _authApiService = authApiService;
        _authStateService = authStateService;
        _toastManager = toastManager;

        _authStateService.StateChanged += OnAuthStateChanged;
    }

    /// <summary>
    /// Загружает актуальные данные пользователя при открытии страницы
    /// </summary>
    public override Task InitializeAsync() => LoadUserAsync();

    #region Methods
    /// <summary>
    /// Реакция на вход или выход пользователя, обновляет признак авторизации и перезагружает данные
    /// </summary>
    private void OnAuthStateChanged()
    {
        OnPropertyChanged(nameof(IsAuthenticated));
        _ = LoadUserAsync();
    }

    /// <summary>
    /// Загружает данные пользователя из /api/auth/me. Предыдущая незавершённая загрузка отменяется.
    /// При ошибке показывает уведомление и подставляет данные, полученные при входе, чтобы страница не осталась пустой
    /// </summary>
    private async Task LoadUserAsync()
    {
        var previous = _loadCts;
        var cts = _loadCts = new CancellationTokenSource();

        previous?.Cancel();
        previous?.Dispose();

        if (!IsAuthenticated)
        {
            User = null;
            return;
        }

        var response = await _authApiService.GetMeAsync(cts.Token);

        if (cts.IsCancellationRequested || IsDisposed)
            return;

        if (response.Success)
        {
            User = response.Data;
            return;
        }

        _toastManager.CreateToast("Загрузка данных").WithContent($"Не удалось обновить данные: {response.ErrorMessage}").ShowError();
        User = _authStateService.CurrentUser;
    }

    /// <summary>
    /// Отписывается от событий долгоживущего сервиса и отменяет незавершённую загрузку
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _authStateService.StateChanged -= OnAuthStateChanged;

            _loadCts?.Cancel();
            _loadCts?.Dispose();
            _loadCts = null;
        }

        base.Dispose(disposing);
    }
    #endregion
}