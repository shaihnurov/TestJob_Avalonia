using System;
using TestJob_Avalonia.Models.Dto;

namespace TestJob_Avalonia.Services.Auth;

/// <summary>
/// Хранит текущее состояние авторизации пользователя в рамках сессии приложения
/// </summary>
public interface IAuthStateService
{
    /// <summary>
    /// Текущий access токен для авторизации запросов
    /// </summary>
    string? AccessToken { get; }

    /// <summary>
    /// Текущий refresh токен, полученный при логине
    /// </summary>
    string? RefreshToken { get; }

    /// <summary>
    /// Данные текущего авторизованного пользователя
    /// </summary>
    UserDto? CurrentUser { get; }

    /// <summary>
    /// true, если пользователь авторизован (есть и токен, и данные пользователя)
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Вызывается при любом изменении состояния авторизации (вход, выход, обновление профиля)
    /// </summary>
    event Action? StateChanged;

    /// <summary>
    /// Устанавливает состояние авторизованной сессии на основе результата успешного логина
    /// </summary>
    /// <param name="authResult">Ответ эндпоинта /login (токены+данные пользователя)</param>
    void SetSession(AuthResultDto authResult);

    /// <summary>
    /// Сбрасывает состояние авторизации /logout
    /// </summary>
    void Clear();
}