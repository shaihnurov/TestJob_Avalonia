using System.Threading;
using System.Threading.Tasks;

namespace TestJob_Avalonia.Services.Http;

/// <summary>
/// Контракт HTTP клиента для выполнения запросов к REST API
/// </summary>
public interface IHttpService
{
    /// <summary>
    /// Выполняет GET запрос и десериализует тело успешного ответа
    /// </summary>
    /// <typeparam name="T">Ожидаемый тип данных</typeparam>
    /// <param name="endpoint">Относительный путь</param>
    /// <param name="ct">Токен отмены</param>
    Task<ApiResult<T>> GetAsync<T>(string endpoint, CancellationToken ct = default);

    /// <summary>
    /// Выполняет POST запрос с телом <paramref name="body"/> и десериализует ответ
    /// </summary>
    /// <typeparam name="T">Ожидаемый тип данных</typeparam>
    /// <param name="endpoint">Относительный путь</param>
    /// <param name="body">Тело запроса, будет сериализовано в JSON</param>
    /// <param name="ct">Токен отмены</param>
    Task<ApiResult<T>> PostAsync<T>(string endpoint, object body, CancellationToken ct = default);
}