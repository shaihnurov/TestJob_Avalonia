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
using TestJob_Avalonia.Services.Logging;
using TestJob_Avalonia.Services.Navigations;
using TestJob_Avalonia.ViewModels;
using TestJob_Avalonia.Views;

namespace TestJob_Avalonia;

public partial class App : Application
{
    private IHost? _host;
    private ILogger<App>? _logger;
#if RELEASE
        private static Mutex? _appMutex;
#endif

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
#if DEBUG
        this.AttachDeveloperTools();
#endif
    }

    public override async void OnFrameworkInitializationCompleted()
    {
#if RELEASE
            _appMutex = new Mutex(true, "TestJob_AvaloniaSingleInstanceMutex", out var createdNew);
            if (!createdNew)
            {
/*                var instanceDialog = new InstanceDialog();
                instanceDialog.Show();
                return;*/
            }
#endif
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
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Debug)
            .Enrich.FromLogContext()
            .Enrich.With<ShortSourceContextEnricher>()
            .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                logsPath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                outputTemplate:
                    "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
#else
            string logsPath = Path.Combine(AppPaths.LogFolder, "main-.log");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Error)
                .Enrich.WithMachineName()
                .Enrich.WithProcessId()
                .Enrich.WithThreadId()
                .WriteTo.File(
                    logsPath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    outputTemplate:
                    "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj} " +
                    "{ErrorType} {ErrorId} {ErrorMessage}{NewLine}")
                .CreateLogger();
#endif

        _host.Services.GetRequiredService<ILoggerFactory>().AddSerilog();
        _logger = _host.Services.GetRequiredService<ILogger<App>>();

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

    /// <summary>
    /// Создает необходимые папки приложения
    /// </summary>
    private static void EnsureFoldersExist()
    {
        Directory.CreateDirectory(AppPaths.LogFolder);
    }
}