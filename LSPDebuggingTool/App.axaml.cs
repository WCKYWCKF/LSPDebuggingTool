using System;
using System.Globalization;
using Akavache;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using HotAvalonia;
using LSPDebuggingTool.Models;
using LSPDebuggingTool.ViewModels;
using LSPDebuggingTool.Views;
using Serilog;
using Ursa.Controls;

namespace LSPDebuggingTool
{
    public class App : Application
    {
        public override void Initialize()
        {
            this.EnableHotReload();
            AvaloniaXamlLoader.Load(this);
        }

        public event Action? AppExit;

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;
                desktop.MainWindow = new InitialWindow();

                desktop.Exit += (sender, args) =>
                {
                    AppExit?.Invoke();
                    BlobCache.Shutdown().Wait();
                    Log.Information("软件已正常关闭");
                };
            }

            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
            {
                singleViewPlatform.MainView = new UrsaView() { Content = new MainPageView() };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}