using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using LSPDebuggingTool.Models;
using LSPDebuggingTool.ViewModels;
using Serilog;
using Ursa.Controls;

namespace LSPDebuggingTool.Views;

public partial class InitialWindow : SplashWindow
{
    public InitialWindow()
    {
        InitializeComponent();
        Close();
    }

    protected override Task<Window?> CreateNextWindow()
    {
        return Task.Run(Action)!;

        Window Action()
        {
            Akavache.Registrations.Start(nameof(LSPDebuggingTool));
            SettingsStorage.Initialize();
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(Path.Combine(SettingsStorage.Instance!.LogPath!, "log.txt"),
                    rollingInterval: RollingInterval.Day)
                .CreateLogger();
            MainWindow mainWindow = Dispatcher.UIThread.Invoke(() => new MainWindow());
            MainPageViewModel viewModel = new();
            Dispatcher.UIThread.Post(() => mainWindow.ViewModel = viewModel);
            Thread.Sleep(TimeSpan.FromSeconds(2));
            Log.Information("Model（MVVM）运行时信息已就绪。");
            return mainWindow;
        }
    }
}