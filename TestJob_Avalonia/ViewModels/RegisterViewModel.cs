using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShadUI;
using TestJob_Avalonia.Models.Request;
using TestJob_Avalonia.Services.Attributes;
using TestJob_Avalonia.Services.Auth;
using TestJob_Avalonia.Services.Auth.API;

namespace TestJob_Avalonia.ViewModels;

/// <summary>
/// ViewModel диалога регистрации
/// </summary>
public partial class RegisterViewModel(DialogManager dialogManager, ToastManager toastManager, IAuthApiService authApiService, IAuthStateService authStateService) : ViewModelBase
{
    #region Свойства
    /// <summary>
    /// Email пользователя
    /// </summary>
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    [Required(ErrorMessage = "Email обязательный")]
    [EmailValidation]
    public partial string Email { get; set; } = string.Empty;

    /// <summary>
    /// Пароль пользователя
    /// </summary>
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    [Required(ErrorMessage = "Пароль обязательный")]
    [MinLength(8, ErrorMessage = "Длина пароля должна составлять не менее 8 символов")]
    public partial string Password { get; set; } = string.Empty;

    /// <summary>
    /// Отображаемое имя пользователя
    /// </summary>
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    [Required(ErrorMessage = "Имя обязательное")]
    public partial string Name { get; set; } = string.Empty;

    /// <summary>
    /// Индикатор выполнения запроса (прогресс на кнопке)
    /// </summary>
    [ObservableProperty]
    public partial bool IsBusy { get; set; }
    #endregion

    #region Методы
    /// <summary>
    /// Регистрирует пользователя, затем выполняет автоматический вход и закрывает диалог
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanSubmit), AllowConcurrentExecutions = false)]
    private async Task Submit()
    {
        IsBusy = true;
        try
        {
            var registerResult = await authApiService.RegisterAsync(new RegisterRequest(Email, Password, Name));
            if (!registerResult.Success)
            {
                toastManager.CreateToast("Регистрация").WithContent($"Ошибка регистрации: {registerResult.ErrorMessage}").ShowError();
                return;
            }

            var loginResult = await authApiService.LoginAsync(new LoginRequest(Email, Password));
            if (!loginResult.Success)
            {
                toastManager.CreateToast("Авторизация").WithContent($"Регистрация выполнена, но войти автоматически не удалось: {loginResult.ErrorMessage}").ShowWarning();
                dialogManager.Close(this, new CloseDialogOptions { Success = true });
                return;
            }

            authStateService.SetSession(loginResult.Data!);
            dialogManager.Close(this, new CloseDialogOptions { Success = true });
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Кнопка доступна, когда все поля заполнены и нет ошибок валидации
    /// </summary>
    private bool CanSubmit() => !HasErrors && !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Password);
    #endregion
}