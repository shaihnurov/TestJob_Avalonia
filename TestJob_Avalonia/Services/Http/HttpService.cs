using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TestJob_Avalonia.Services.Auth;

namespace TestJob_Avalonia.Services.Http;

/// <summary>
/// Кроссплатформенный сервис для выполнения HTTP-запросов к REST API
/// </summary>
public sealed class HttpService(HttpClient httpClient, ILogger<HttpService> logger, IAuthStateService authState, IOptions<HttpServiceOptions> options) : IHttpService
{
    private readonly HttpServiceOptions _options = options.Value;

    /// <summary>
    /// Настройки сериализации JSON
    /// </summary>
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    /// <inheritdoc />
    public Task<ApiResult<T>> GetAsync<T>(string endpoint, CancellationToken ct = default)
        => SendAsync<T>(HttpMethod.Get, endpoint, ct: ct);

    /// <inheritdoc />
    public Task<ApiResult<T>> PostAsync<T>(string endpoint, object body, CancellationToken ct = default)
        => SendAsync<T>(HttpMethod.Post, endpoint, body, ct: ct);

    /// <summary>
    /// Единая точка отправки всех HTTP запросов: строит запрос, проставляет bearer токен
    /// текущей сессии, сериализует тело и обрабатывает ответ
    /// </summary>
    /// <typeparam name="T">Тип ожидаемых данных в теле успешного ответа</typeparam>
    /// <param name="method">HTTP метод запроса</param>
    /// <param name="endpoint">Относительный путь endpoint'a</param>
    /// <param name="body">Тело запроса для сериализации в JSON</param>
    /// <param name="ct">Токен отмены</param>
    private async Task<ApiResult<T>> SendAsync<T>(HttpMethod method, string endpoint, object? body = null, CancellationToken ct = default)
    {
        try
        {
            using var request = new HttpRequestMessage(method, BuildUrl(endpoint));

            if (authState.AccessToken is { } token)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (body is not null)
                request.Content = JsonContent.Create(body, options: SerializerOptions);

            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
            return await ProcessResponseAsync<T>(response, ct);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            return ApiResult<T>.Fail("Запрос отменён");
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Ошибка подключения к серверу");
            return ApiResult<T>.Fail("Ошибка подключения к серверу");
        }
    }

    /// <summary>
    /// Читает тело ответа и преобразует его в <see cref="ApiResult{T}"/>
    /// </summary>
    private async Task<ApiResult<T>> ProcessResponseAsync<T>(HttpResponseMessage response, CancellationToken ct)
    {
        var body = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = ExtractErrorMessage(body);
            logger.LogError("[{StatusCode}]: {ErrorMessage}", response.StatusCode, errorMessage);
            return ApiResult<T>.Fail(errorMessage);
        }

        try
        {
            var data = JsonSerializer.Deserialize<T>(body, SerializerOptions);
            return data is not null ? ApiResult<T>.Ok(data) : ApiResult<T>.Fail("Пустой ответ от сервера");
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Ошибка десериализации JSON: {Body}", body);
            return ApiResult<T>.Fail("Ошибка десериализации ответа от сервера");
        }
    }

    /// <summary>
    /// Строит абсолютный URL запроса, если <paramref name="endpoint"/> уже абсолютный использует его как есть,
    /// иначе дописывает базовый адрес из <see cref="HttpServiceOptions"/>
    /// </summary>
    private string BuildUrl(string endpoint) =>
        Uri.IsWellFormedUriString(endpoint, UriKind.Absolute) ? endpoint : $"{_options.BaseUrl}{endpoint}";

    /// <summary>
    /// Пытается вытащить человекочитаемое сообщение об ошибке из тела не успешного ответа
    /// </summary>
    private static string ExtractErrorMessage(string? responseBody)
    {
        if (string.IsNullOrWhiteSpace(responseBody))
            return "Неизвестная ошибка";

        try
        {
            var node = JsonNode.Parse(responseBody);
            return node?["message"]?.GetValue<string>() ?? node?["error"]?.GetValue<string>() ?? "Ошибка запроса";
        }
        catch
        {
            return responseBody.Length > 200 ? responseBody[..200] + "..." : responseBody;
        }
    }
}