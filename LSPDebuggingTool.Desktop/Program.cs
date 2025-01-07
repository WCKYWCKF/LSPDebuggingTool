using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Runtime.Versioning;
using System.Threading;
using Avalonia;
using Avalonia.Dialogs;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using DynamicData;
using DynamicData.Alias;
using DynamicData.Binding;
using ReactiveUI;

namespace LSPDebuggingTool.Desktop;

[SupportedOSPlatform("windows")]
[SupportedOSPlatform("linux")]
[SupportedOSPlatform("macOS")]
internal sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .UseManagedSystemDialogs()
            .WithInterFont()
            .UseReactiveUI()
            .LogToTrace();
    }
}