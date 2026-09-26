using System;
using TestJob_Avalonia.Models.Dto;

namespace TestJob_Avalonia.Services.Auth;

/// <inheritdoc cref="IAuthStateService"/>
public sealed class AuthStateService : IAuthStateService
{
    #region Properties
    /// <inheritdoc />
    public string? AccessToken { get; private set; }

    /// <inheritdoc />
    public string? RefreshToken { get; private set; }

    /// <inheritdoc />
    public UserDto? CurrentUser { get; private set; }

    /// <inheritdoc />
    public bool IsAuthenticated => AccessToken is not null && CurrentUser is not null;

    /// <inheritdoc />
    public event Action? StateChanged;
    #endregion

    /// <inheritdoc />
    public void SetSession(AuthResultDto authResult)
    {
        AccessToken = authResult.AccessToken;
        RefreshToken = authResult.RefreshToken;
        CurrentUser = authResult.User;
        StateChanged?.Invoke();
    }

    /// <inheritdoc />
    public void SetCurrentUser(UserDto user)
    {
        CurrentUser = user;
        StateChanged?.Invoke();
    }

    /// <inheritdoc />
    public void Clear()
    {
        AccessToken = null;
        RefreshToken = null;
        CurrentUser = null;
        StateChanged?.Invoke();
    }
}