using Avalonia;
using Avalonia.Controls;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using Avalonia.Controls.ApplicationLifetimes;
using System.Threading;
using System.Threading.Tasks;

namespace BitDesk;

internal sealed class Program
{
    private const string MutexName = "SingleInstanceMutexForBitDesk";
    private const string PipeName = "BitDesk.SingleInstance.Pipe";
    private static Mutex? _mutex;

    private static string GetPipePath()
    {
        var tempPath = System.IO.Path.GetTempPath();
        return System.IO.Path.Combine(tempPath, PipeName);
    }

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // First, use a mutex to ensure thread-safe access to the pipe logic.
            _mutex = new Mutex(true, MutexName, out bool isNewInstance);
            if (!isNewInstance)
            {
                // Second instance: send a message to the first instance and exit.
                //SendFocusCommandToExistingInstance();

                // The mutex is already owned, indicating another instance is running.
                HandleExistingInstance(args);

                return;
            }
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            var pipePath = GetPipePath();
            if (File.Exists(pipePath))
            {
                // Check that the first instance is actually alive by connecting to it.
                if (HandleExistingInstance(args))
                {
                    // The running instance was notified. Nothing more to do.
                    return;
                }

                // Stale pipe file (previous instance crashed). Remove it and start normally.
                try
                {
                    File.Delete(pipePath);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Could not delete stale pipe file: {ex.Message}");
                    _ = ex; // For native code analysis to avoid warning about unused variable.
                }
            }
        }

        // First instance: start the application and listen for pipe messages.
        StartPipeServer(args);

        try
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
        }
        finally
        {
            //_pipeCancellationTokenSource.Cancel();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _mutex?.ReleaseMutex();
                //_mutex.Dispose();
            }
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    
    private static void OnProcessExit(object? sender, EventArgs e)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || !File.Exists(GetPipePath()))
        {
            return;
        }
        try
        {
            File.Delete(GetPipePath());
        }
        catch (Exception ex)
        {
            // Log the error.
            Debug.WriteLine($"Error deleting named pipe: {ex.Message}");
            _ = ex;// For native code analysis to avoid warning about unused variable.
        }
    }

    private static void StartPipeServer(string[] args)
    {
        var pipePath = GetPipePath();

        // Ensure the previous pipe is deleted in case of a crash.
        try
        {
            if (File.Exists(pipePath))
            {
                File.Delete(pipePath);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Could not delete previous pipe file: {ex.Message}");
            _ = ex;
        }

        Task.Run(() =>
        {
            while (true)
            {
                try
                {
                    // Create a named pipe for incoming connections.
                    var server = new NamedPipeServerStream(pipePath, PipeDirection.In);
                    server.WaitForConnection();

                    // Read messages from the new instance.
                    using (var reader = new StreamReader(server))
                    {
                        string? line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            // Handle the message on the UI thread.
                            if (line != "ShowMainWindow" || Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
                            {
                                continue;
                            }
                            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                            {
                                var mainWnd = desktop.MainWindow;
                                if (mainWnd is null)
                                {
                                    return;
                                }
                                if (mainWnd.WindowState == WindowState.Minimized)
                                {
                                    mainWnd.WindowState = WindowState.Normal;
                                }
                                mainWnd.Show();
                                mainWnd.Activate();
                            });
                        }
                    }

                    //server.Disconnect();
                    if (server.IsConnected)
                    {
                        server.Disconnect();
                    }
                }
                catch (Exception ex)
                {
                    _ = ex; // For native code analysis to avoid warning about unused variable.
                    Debug.WriteLine($"Exception @StartPipeServer while: {ex}");
                    // Just break out the loop in case of exception.
                    break;
                }
            }
        });
    }

    private static bool HandleExistingInstance(string[] args)
    {
        var pipePath = GetPipePath();

        // Attempt to connect to the named pipe.
        using var client = new NamedPipeClientStream(".", pipePath, PipeDirection.Out);
        try
        {
            // Give it a brief timeout to find the server.
            client.Connect(1000);
            if (client.IsConnected)
            {
                // Send a message to the first instance.
                using var writer = new StreamWriter(client);
                writer.WriteLine("ShowMainWindow");
                foreach (var arg in args)
                {
                    writer.WriteLine(arg);
                }
                writer.Flush();

                return true;
            }
            else
            {
                return false;
            }
        }
        catch (TimeoutException)
        {
            // First instance did not respond, it may have crashed.
            return false;
        }
        catch (Exception ex)
        {
            // Handle other potential pipe communication errors.
            Debug.WriteLine($"Could not connect to existing instance: {ex.Message}");
            _ = ex; // For native code analysis to avoid warning about unused variable.
            return false;
        }
    }
}
