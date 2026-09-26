namespace TestJob_Avalonia.Services.Http;

/// <summary>
/// Настройки HTTP-сервиса, загружаемые из конфигурации приложения (appsettings.json)
/// </summary>
public sealed class HttpServiceOptions
{
    /// <summary>
    /// Базовый URL сервера API
    /// </summary>
    public string BaseUrl { get; init; } = string.Empty;
}