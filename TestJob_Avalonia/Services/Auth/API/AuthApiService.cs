using System.Threading;
using System.Threading.Tasks;
using TestJob_Avalonia.Models.Dto;
using TestJob_Avalonia.Models.Request;
using TestJob_Avalonia.Services.Http;

namespace TestJob_Avalonia.Services.Auth.API;

/// <inheritdoc cref="IAuthApiService"/>
public sealed class AuthApiService(IHttpService httpService) : IAuthApiService
{
    /// <inheritdoc />
    public async Task<ApiResult<UserDto>> GetMeAsync(CancellationToken ct = default)
        => await httpService.GetAsync<UserDto>("/api/auth/me", ct);

    /// <inheritdoc />
    public async Task<ApiResult<UserDto>> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
        => await httpService.PostAsync<UserDto>("/api/auth/register", request, ct);

    /// <inheritdoc />
    public async Task<ApiResult<AuthResultDto>> LoginAsync(LoginRequest request, CancellationToken ct = default)
        => await httpService.PostAsync<AuthResultDto>("/api/auth/login", request, ct);

    /// <inheritdoc />
    public async Task<ApiResult<LogoutResultDto>> LogoutAsync(LogoutRequest request, CancellationToken ct = default)
        => await httpService.PostAsync<LogoutResultDto>("/api/auth/logout", request, ct);
}