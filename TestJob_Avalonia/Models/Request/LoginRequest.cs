namespace TestJob_Avalonia.Models.Request;

/// <summary>
/// Тело запроса POST /api/auth/login
/// </summary>
/// <param name="Email">Email</param>
/// <param name="Password">Пароль</param>
public sealed record LoginRequest(string Email, string Password);