namespace TestJob_Avalonia.Services.Http;

/// <summary>
/// Результат выполнения HTTP запроса. Инкапсулирует три исхода: успешное выполнение с данными,
/// не успешное выполнение с сообщением об ошибке, и позволяет вызывающему коду не заворачивать каждый вызов 
/// <see cref="IHttpService"/> в try/catch на ожидаемые бизнес-ошибки
/// </summary>
/// <typeparam name="T">Тип полезных данных при успешном результате</typeparam>
/// <param name="Success">Признак успешного выполнения запроса</param>
/// <param name="Data">Данные ответа при успехе, default при ошибке</param>
/// <param name="ErrorMessage">Сообщение об ошибке, null при успешном выполнении</param>
public readonly record struct ApiResult<T>(bool Success, T? Data, string? ErrorMessage)
{
    /// <summary>
    /// Создаёт успешный результат с данными ответа
    /// </summary>
    public static ApiResult<T> Ok(T data) => new(true, data, null);

    /// <summary>
    /// Создаёт провальный результат с сообщением об ошибке
    /// </summary>
    public static ApiResult<T> Fail(string errorMessage) => new(false, default, errorMessage);
}