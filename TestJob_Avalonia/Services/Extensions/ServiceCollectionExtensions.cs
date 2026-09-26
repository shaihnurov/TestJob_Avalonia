using System.ComponentModel.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestJob_Avalonia.Services.Auth;
using TestJob_Avalonia.Services.Http;
using TestJob_Avalonia.Services.Navigations;
using TestJob_Avalonia.ViewModels;

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

        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IAuthStateService, AuthStateService>();

        return services;
    }

    /// <summary>
    /// Регистрация сервисов с жизненным циклом Transient.
    /// </summary>
    public static IServiceCollection AddTransientServices(this IServiceCollection services)
    {
        

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
}