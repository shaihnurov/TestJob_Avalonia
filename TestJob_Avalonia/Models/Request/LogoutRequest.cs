namespace TestJob_Avalonia.Models.Request;

/// <summary>
/// Тело запроса POST /api/auth/logout
/// </summary>
/// <param name="RefreshToken">Refresh токен текущей сессии</param>
public sealed record LogoutRequest(string RefreshToken);