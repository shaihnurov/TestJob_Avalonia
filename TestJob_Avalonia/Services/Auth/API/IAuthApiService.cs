using System.Threading;
using System.Threading.Tasks;
using TestJob_Avalonia.Models.Dto;
using TestJob_Avalonia.Models.Request;
using TestJob_Avalonia.Services.Http;

namespace TestJob_Avalonia.Services.Auth.API;

/// <summary>
/// Контракт для взаимодействия с REST API авторизации
/// </summary>
public interface IAuthApiService
{
    /// <summary>
    /// Выполняет GET запрос на эндпоинт /api/auth/me
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Данные пользователя</returns>
    Task<ApiResult<UserDto>> GetMeAsync(CancellationToken ct = default);

    /// <summary>
    /// Выполняет POST запрос на эндпоинт /api/auth/register
    /// </summary>
    /// <param name="request">Запрос на регистрацию</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Данные пользователя</returns>
    Task<ApiResult<UserDto>> RegisterAsync(RegisterRequest request, CancellationToken ct = default);

    /// <summary>
    /// Выполняет POST запрос на эндпоинт /api/auth/login
    /// </summary>
    /// <param name="request">Запрос на авторизацию</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Результат авторизации</returns>
    Task<ApiResult<AuthResultDto>> LoginAsync(LoginRequest request, CancellationToken ct = default);

    /// <summary>
    /// Выполняет POST запрос на эндпоинт /api/auth/logout
    /// </summary>
    /// <param name="request">Запрос на выход</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Результат выхода из системы</returns>
    Task<ApiResult<LogoutResultDto>> LogoutAsync(LogoutRequest request, CancellationToken ct = default);
}