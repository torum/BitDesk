using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using BitDesk.Services;
using BitDesk.Services.Contracts;
using BitDesk.ViewModels;
using BitDesk.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BitDesk;

public partial class App : Application
{
    public const string AppName = "BitDesk";
    private const string AppDeveloper = "torum";

    // Log file.
    private static readonly StringBuilder _errortxt = new();
    private static readonly Lock _logLock = new();
    private static readonly string _logFilePath = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + System.IO.Path.DirectorySeparatorChar + AppName + "_errors.txt";

    // Data folder and Config file path.
    private static readonly string _envDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    public static string AppDataFolder { get; } = System.IO.Path.Combine(System.IO.Path.Combine(_envDataFolder, AppDeveloper), AppName);
    public static string AppConfigFilePath { get; } = System.IO.Path.Combine(AppDataFolder, AppName + ".config");

    public App()
    {
        var services = new ServiceCollection();

        services.AddSingleton<MainWindow>();
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<IBitbankStreamService, BitbankStreamService>();
        services.AddSingleton<IConfigurationService, ConfigurationService>();
        services.AddSingleton<INavigationService, NavigationService>();

        services.AddHttpClient("BitDesk", client =>
        {
            // Addition to this timeout, also use the per-request cancellation token as the fallback timeout mechanism.
            // Though, not a good idea to set timeout here to avoid disposable race. 
            client.Timeout = TimeSpan.FromSeconds(13);//Timeout.InfiniteTimeSpan;//
            // Preemptively close the connection to avoid IOExceptions caused by remote server silently tears down an idle TLS/socket connection.
            // .NET's internal reading loops encounter expected IOExceptions or SocketExceptions on server side closed idle keep-alive sockets.
            client.DefaultRequestHeaders.ConnectionClose = false;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            client.DefaultRequestHeaders.UserAgent.ParseAdd("FeedDesk/1.2");
        }).ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
        {
            ConnectTimeout = TimeSpan.FromSeconds(8),
            PooledConnectionLifetime = TimeSpan.FromMinutes(5),
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(1),
            SslOptions = new System.Net.Security.SslClientAuthenticationOptions
            {
                // Optional: nothing to change here by default; handshake time is bounded by ConnectTimeout.
            }
        });
        services.AddSingleton<IBitbankHttpService, BitbankHttpService>();

        // Build the provider
        ServiceProvider = services.BuildServiceProvider();
    }

    public IServiceProvider ServiceProvider { get; private set; } = null!;

    public static T GetService<T>() where T : class
    {
        return (App.Current as App)!.ServiceProvider.GetRequiredService<T>();
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public async override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            RegisterGlobalExceptionHandlers();

            // Set custom font for non-Windows platforms.
            if (Application.Current != null)
            {
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Application.Current.Resources["ContentControlThemeFontFamily"] = new FontFamily("fonts:Inter#Inter");
                }
            }

            var config = ServiceProvider.GetRequiredService<IConfigurationService>();
            config.Initialize(AppConfigFilePath);
            await config.LoadConfigurationAsync();

            var mainWin = App.GetService<MainWindow>();
            desktop.MainWindow = mainWin;

        }

        base.OnFrameworkInitializationCompleted();
    }

    private void RegisterGlobalExceptionHandlers()
    {
        // UI thread.
        Dispatcher.UIThread.UnhandledException += OnUnhandledException;

        // Any thread, process is about to terminate.
        AppDomain.CurrentDomain.UnhandledException += OnDomainUnhandledException;

        // Faulted tasks nobody awaited.
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
    }

    private void OnUnhandledException(object? sender, DispatcherUnhandledExceptionEventArgs e)
    {
        // Cancellation is normal (e.g. shutdown), not an error.
        if (e.Exception is OperationCanceledException)
        {
            e.Handled = true;
            return;
        }

        System.Diagnostics.Debug.WriteLine($"An unhandled exception occurred: {e.Exception}");
        AppendErrorLog("DispatcherUnhandledException", e.Exception.ToString());
        SaveErrorLog();

        // Keep the app alive for recoverable errors, but never for out-of-memory.
        e.Handled = e.Exception is not OutOfMemoryException;
    }

    private static void OnDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is not Exception exception)
        {
            var raw = e.ExceptionObject?.ToString() ?? "Unknown exception object";
            Debug.WriteLine($"CurrentDomain_UnhandledException: {raw}");
            AppendErrorLog("CurrentDomain_UnhandledException", raw);
            SaveErrorLog();
            return;
        }

        if (exception is TaskCanceledException)
        {
            // can ignore.
            Debug.WriteLine($"CurrentDomain_UnhandledException (TaskCanceledException): {exception.Message}");
            return;
        }

        AppendErrorLog("AppDomain.UnhandledException", e.ExceptionObject?.ToString() ?? "(null)");
        SaveErrorLog(); // Last chance: the process terminates after this.
    }

    private static void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        if (e.Exception.InnerExceptions.All(ex => ex is OperationCanceledException))
        {
            e.SetObserved();
            return;
        }

        var inner = e.Exception.Flatten().InnerException;
        // Check if the exception originates from a network drop or SocketIO background task
        if (inner is System.Net.Http.HttpRequestException ||
            inner is System.Net.Sockets.SocketException ||
            inner is System.Net.WebSockets.WebSocketException ||
            e.Exception.StackTrace?.Contains("SocketIOClient") == true)
        {
            System.Diagnostics.Debug.WriteLine($"[Suppressed Network Background Exception]: {inner?.Message}");

            e.SetObserved();
            return;
        }

        AppendErrorLog("TaskScheduler.UnobservedTaskException", e.Exception.ToString());
        SaveErrorLog();
        e.SetObserved();
    }

    public static void AppendErrorLog(string source, string details)
    {
        var nowString = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);

        lock (_logLock)
        {
            _errortxt.AppendLine(nowString + " - " + source + " - " + details);
        }
    }

    public static void SaveErrorLog()
    {
        if (!App.GetService<MainViewModel>()?.IsSaveLog ?? false)
            return;

        if (string.IsNullOrEmpty(_logFilePath))
            return;

        try
        {
            lock (_logLock)
            {
                if (_errortxt.Length == 0)
                {
                    return;
                }

                Directory.CreateDirectory(AppDataFolder);
                File.WriteAllText(_logFilePath, _errortxt.ToString());
            }
        }
        catch (Exception ex)
        {
            // Logging must never take the app down.
            System.Diagnostics.Debug.WriteLine($"Could not save error log: {ex.Message}");
        }
    }
}