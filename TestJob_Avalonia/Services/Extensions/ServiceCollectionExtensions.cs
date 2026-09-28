using System;
using Avalonia;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShadUI;
using TestJob_Avalonia.Services.Auth;
using TestJob_Avalonia.Services.Auth.API;
using TestJob_Avalonia.Services.Factory;
using TestJob_Avalonia.Services.Http;
using TestJob_Avalonia.Services.Navigations;
using TestJob_Avalonia.ViewModels;
using TestJob_Avalonia.Views;

namespace TestJob_Avalonia.Services.Extensions;

/// <summary>
/// Методы-расширения для регистрации сервисов в контейнере зависимостей
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Регистрация сервисов с жизненным циклом Singleton.
    /// </summary>
    public static IServiceCollection AddSingletonServices(this IServiceCollection services)
    {
        services.AddViewModel<MainWindowViewModel>(ServiceLifetime.Singleton);
        services.AddViewModel<AuthViewModel>(ServiceLifetime.Singleton);

        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IAuthStateService, AuthStateService>();
        services.AddSingleton<DialogManager>();
        services.AddSingleton<ToastManager>();
        services.AddSingleton(sp => new ThemeWatcher(Application.Current!));

        return services;
    }

    /// <summary>
    /// Регистрация сервисов с жизненным циклом Transient.
    /// </summary>
    public static IServiceCollection AddTransientServices(this IServiceCollection services)
    {
        services.AddTransient(typeof(IViewModelFactory<>), typeof(ViewModelFactory<>));

        services.AddViewModel<UserInfoViewModel>(ServiceLifetime.Transient);
        services.AddViewModel<DevInfoViewModel>(ServiceLifetime.Transient);
        services.AddViewModel<RegisterViewModel>(ServiceLifetime.Transient);

        services.AddTransient<IAuthApiService, AuthApiService>();

        return services;
    }

    /// <summary>
    /// Регистрация общих библиотечных сервисов.
    /// </summary>
    public static IServiceCollection AddCommonServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IHttpService, HttpService>();
        services.Configure<HttpServiceOptions>(configuration.GetSection("HttpService"));

        return services;
    }

    private static void AddViewModel<T>(this IServiceCollection services, ServiceLifetime lifetime) where T : class
    {
        var descriptor = new ServiceDescriptor(typeof(T), typeof(T), lifetime);
        services.Add(descriptor);
        ViewModelLifetimeRegistry.Register(typeof(T), lifetime);
    }

    public static IServiceProvider RegisterDialogs(this IServiceProvider serviceProvider)
    {
        var dialogManager = serviceProvider.GetRequiredService<DialogManager>();

        dialogManager.Register<RegisterView, RegisterViewModel>();

        return serviceProvider;
    }
}