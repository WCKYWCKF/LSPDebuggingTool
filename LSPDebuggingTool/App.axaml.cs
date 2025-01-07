using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DynamicData;
using DynamicData.Binding;
using LSPDebuggingTool.Models;
using LSPDebuggingTool.ViewModels;
using LSPDebuggingTool.Views;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;

namespace LSPDebuggingTool.Models
{
    public sealed partial class SettingStorage
    {
        public AppCulture CurrentCulture
        {
            get;
            set => SetField(ref field, value);
        } = AppCulture.Chinese;
    }

    public enum AppCulture
    {
        English,
        Chinese
    }
}


namespace LSPDebuggingTool
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            if (SettingStorage.Instance.CurrentCulture is AppCulture.English)
                Localization.Resources.Culture = new CultureInfo("en");
        }

        public event Action? AppExit;

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;
                desktop.MainWindow = new MainWindow
                {
                    ViewModel = MainWindowViewModel.Init()
                };

                desktop.Exit += (sender, args) =>
                {
                    AppExit?.Invoke();
                    RuntimeBasicInfo.Log.Information("软件已正常关闭");
                };
            }

            // else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
            // {
            //     singleViewPlatform.MainView = new MainView
            //     {
            //         DataContext = new MainViewModel()
            //     };
            // }
            RuntimeBasicInfo.Log.Information("软件已正常启动");
            base.OnFrameworkInitializationCompleted();
        }
    }
}