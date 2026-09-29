using System;
using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using TestJob_Avalonia.Services.Common;
using TestJob_Avalonia.Services.Extensions;
using TestJob_Avalonia.ViewModels;
using TestJob_Avalonia.Views;
using Avalonia.Threading;

#if DEBUG
using TestJob_Avalonia.Services.Logging;
#endif

namespace TestJob_Avalonia;

public partial class App : Application
{
    /// <summary>
    /// Generic Host приложения — единая точка сборки DI-контейнера, конфигурации (appsettings.json) и логирования
    /// </summary>
    private IHost? _host;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        Dispatcher.UIThread.UnhandledException += OnUnhandledException;
#if DEBUG
        this.AttachDeveloperTools();
#endif
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        try
        {
            EnsureFoldersExist();

            _host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(AppContext.BaseDirectory);
#if RELEASE
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
#else
                    config.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
#endif
                })
                .ConfigureServices((ctx, services) =>
                {
                    services.AddCommonServices(ctx.Configuration);
                    services.AddTransientServices();
                    services.AddSingletonServices();
                }).Build();

#if DEBUG
            string logsPath = Path.Combine(AppPaths.LogFolder, "debug-.log");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
                .Enrich.FromLogContext()
                .Enrich.With<ShortSourceContextEnricher>()
                .WriteTo.Async(a =>
                {
                    a.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}");
                    a.File(logsPath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7,
                        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}");
                })
                .CreateLogger();
#else
            string logsPath = Path.Combine(AppPaths.LogFolder, "main-.log");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.WithMachineName()
                .Enrich.WithProcessId()
                .Enrich.WithThreadId()
                .WriteTo.Async(a => a.File(logsPath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj} {NewLine}"))
                .CreateLogger();
#endif

            _host.Services.GetRequiredService<ILoggerFactory>().AddSerilog();
            _host.Services.RegisterDialogs();

            await _host.StartAsync();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainWindow = new MainWindow();
                var mainWindowVm = _host.Services.GetRequiredService<MainWindowViewModel>();

                mainWindow.DataContext = mainWindowVm;
                mainWindow.Show();

                await mainWindowVm.InitializeAsync();
            }

            base.OnFrameworkInitializationCompleted();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Не удалось запустить приложение");
            throw;
        }
    }

    /// <summary>
    /// Создает необходимые папки приложения
    /// </summary>
    private static void EnsureFoldersExist()
    {
        Directory.CreateDirectory(AppPaths.LogFolder);
    }

    /// <summary>
    /// Глобальный перехватчик необработанных исключений UI-потока. Логирует ошибку и пропускает её дальше
    /// </summary>
    private static void OnUnhandledException(object? sender, DispatcherUnhandledExceptionEventArgs e)
        => Log.Error(e.Exception, "Необработанное исключение в UI-потоке");
}