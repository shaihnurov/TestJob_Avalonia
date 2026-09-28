using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShadUI;
using TestJob_Avalonia.Models.Request;
using TestJob_Avalonia.Services.Attributes;
using TestJob_Avalonia.Services.Auth;
using TestJob_Avalonia.Services.Auth.API;
using TestJob_Avalonia.Services.Factory;

namespace TestJob_Avalonia.ViewModels;

/// <summary>
/// ViewModel блока авторизации
/// </summary>
public partial class AuthViewModel : ViewModelBase
{
    #region Depedency
    private readonly IAuthApiService _authApiService;
    private readonly IAuthStateService _authStateService;
    private readonly ToastManager _toastManager;
    private readonly DialogManager _dialogManager;
    private readonly IViewModelFactory<RegisterViewModel> _registerViewModelFactory;
    #endregion

    #region Свойства
    /// <summary>
    /// Email пользователя
    /// </summary>
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [NotifyCanExecuteChangedFor(nameof(AuthCommand))]
    [Required(ErrorMessage = "Email обязательный")]
    [EmailValidation]
    public partial string Email { get; set; } = string.Empty;

    /// <summary>
    /// Пароль пользователя
    /// </summary>
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [NotifyCanExecuteChangedFor(nameof(AuthCommand))]
    [Required(ErrorMessage = "Пароль обязательный")]
    [MinLength(8, ErrorMessage = "Длина пароля должна составлять не менее 8 символов")]
    public partial string Password { get; set; } = string.Empty;

    /// <summary>
    /// Индикатор выполнения запроса (прогресс на кнопке)
    /// </summary>
    [ObservableProperty]
    public partial bool IsBusy { get; set; }

    /// <summary>
    /// Признак того, что пользователь авторизован. Определяет, что показывать в блоке
    /// </summary>
    public bool IsAuthenticated => _authStateService.IsAuthenticated;
    #endregion

    public AuthViewModel(IAuthApiService authApiService, IAuthStateService authStateService, ToastManager toastManager,
        DialogManager dialogManager, IViewModelFactory<RegisterViewModel> registerViewModelFactory)
    {
        _authApiService = authApiService;
        _authStateService = authStateService;
        _toastManager = toastManager;
        _dialogManager = dialogManager;
        _registerViewModelFactory = registerViewModelFactory;

        // Отписываться нет смысла, так как AuthViewModel - Singleton
        _authStateService.StateChanged += () => OnPropertyChanged(nameof(IsAuthenticated));
    }

    #region Методы
    /// <summary>
    /// Выполняет вход
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanSubmit))]
    private async Task Auth()
    {
        IsBusy = true;
        try
        {
            var response = await _authApiService.LoginAsync(new LoginRequest(Email, Password));
            if (!response.Success)
            {
                _toastManager.CreateToast("Авторизация").WithContent($"Ошибка авторизации: {response.ErrorMessage}").ShowError();
                return;
            }

            _authStateService.SetSession(response.Data!);
            ResetForm();
            _toastManager.CreateToast("Авторизация").WithContent("Успешная авторизация").ShowSuccess();
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Завершает сессию, отзывает refresh токен на сервере и очищает локальное состояние
    /// </summary>
    [RelayCommand]
    private async Task Logout()
    {
        if (_authStateService.RefreshToken is { } token)
            await _authApiService.LogoutAsync(new LogoutRequest(token));

        _authStateService.Clear();
    }

    /// <summary>
    /// Открывает диалог регистрации с новым экземпляром <see cref="RegisterViewModel"/>
    /// </summary>
    [RelayCommand]
    private void OpenRegister()
    {
        _dialogManager.CreateDialog(_registerViewModelFactory.Create()).Dismissible()
            .WithSuccessCallback(_ => _toastManager.CreateToast("Регистрация").WithContent("Успешная регистрация").DismissOnClick().ShowSuccess())
            .WithCancelCallback(() => _toastManager.CreateToast("Регистрация").WithContent("Регистрация отменена").DismissOnClick().ShowWarning())
            .Show();
    }

    /// <summary>
    /// Очищает поля формы и сбрасывает ошибки валидации
    /// </summary>
    private void ResetForm()
    {
        Email = string.Empty;
        Password = string.Empty;
        ClearErrors();
    }

    /// <summary>
    /// Кнопка входа доступна, когда оба поля заполнены и нет ошибок валидации
    /// </summary>
    private bool CanSubmit() => !HasErrors && !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password);
    #endregion
}