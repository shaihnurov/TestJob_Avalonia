namespace TestJob_Avalonia.Models.Dto;

/// <summary>
/// Ответ POST /api/auth/logout
/// </summary>
/// <param name="Success">Признак успешного завершения сессии</param>
/// <param name="Message">Сообщение сервера</param>
public sealed record LogoutResultDto(bool Success, string Message);