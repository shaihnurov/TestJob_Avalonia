namespace TestJob_Avalonia.Models.Dto;

/// <summary>
/// Ответ POST /api/auth/login
/// </summary>
/// <param name="AccessToken">Токен</param>
/// <param name="RefreshToken">Токен для обновления сессии и /logout</param>
/// <param name="TokenType">Тип токена</param>
/// <param name="ExpiresIn">Время жизни access токена в секундах</param>
/// <param name="User">Данные авторизованного пользователя</param>
public sealed record AuthResultDto(string AccessToken, string RefreshToken, string TokenType, int ExpiresIn, UserDto User);