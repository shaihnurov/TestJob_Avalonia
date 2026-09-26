namespace TestJob_Avalonia.Models.Request;

/// <summary>
/// Тело запроса POST /api/auth/register
/// </summary>
/// <param name="Email">Email пользователя</param>
/// <param name="Password">Пароль пользователя</param>
/// <param name="Name">Отображаемое имя пользователя</param>
public sealed record RegisterRequest(string Email, string Password, string Name);