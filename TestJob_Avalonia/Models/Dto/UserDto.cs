namespace TestJob_Avalonia.Models.Dto;

/// <summary>
/// Данные пользователя, используемые в UI
/// </summary>
/// <param name="Id">Идентификатор пользователя</param>
/// <param name="Email">Email пользователя</param>
/// <param name="Name">Отображаемое имя</param>
/// <param name="Role">Роль пользователя</param>
/// <param name="Avatar">URL аватарки/param>
public sealed record UserDto(string Id, string Email, string Name, string Role, string? Avatar = null);