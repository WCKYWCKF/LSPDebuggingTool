using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using AvaloniaEdit;
using AvaloniaEdit.Indentation.CSharp;
using AvaloniaEdit.Document;
using AvaloniaEdit.Editing;
using AvaloniaEdit.TextMate;
using LSPDebuggingTool.Models;
using LSPDebuggingTool.ViewModels;
using Material.Icons;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using TextMateSharp.Grammars;
using Ursa.Controls;
using Ursa.ReactiveUIExtension;

namespace LSPDebuggingTool.Views;

public partial class MainWindow : ReactiveUrsaWindow<MainWindowViewModel>
{
    private readonly RegistryOptions _registryOptions = new(ThemeName.LightPlus);

    private readonly TextMate.Installation _textMateInstallation;

    public MainWindow()
    {
        InitializeComponent();
        this.WhenAnyValue(x => x.ViewModel!.LSPClientViewModel!.LogReader.Count)
            .Throttle(TimeSpan.FromSeconds(1))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Do(_ => { _logScrollViewer.ScrollToEnd(); })
            .Subscribe();
        // _textEditor.TextArea.IndentationStrategy =
        //     new CSharpIndentationStrategy();
        _textMateInstallation = _textEditor.InstallTextMate(_registryOptions);
        Caret caret = _textEditor.TextArea.Caret;
        caret.Events().PositionChanged.Subscribe(_ =>
        {
            if (ViewModel?.LSPClientViewModel?.LocationInfo is not { } locationInfo) return;
            locationInfo.LineNumber = caret.Line;
            locationInfo.ColumnNumber = caret.Column;
        });
    }

    private void ArgumentTextBox_OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (sender is not TextBox textBox) return;
        CompositeDisposable disposable = new();
        textBox.Unloaded += (o, args) => disposable.Dispose();
        textBox.Events().LostFocus.Select(args => args.Source is not TextBox textBox1 ? string.Empty : textBox1.Text)
            .InvokeCommand(this, x => x.ViewModel!.LSPClientViewModel!.ArgumentEditCompleteCheckCommand!)
            .DisposeWith(disposable);
        if (string.IsNullOrEmpty(textBox.Text))
        {
            textBox.SelectAll();
            if (textBox.Focus() is false) throw new Exception("获取焦点失败");
        }
    }

    private void Open_Ancillary_Information_File(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button) return;
        var storageProvider = GetTopLevel(this)!.StorageProvider;
        FilePickerOpenOptions pickerOpenOptions = new()
        {
            Title = button.Content as string,
            FileTypeFilter = new List<FilePickerFileType>
            {
                new("附属信息文件") { Patterns = ["*.json"] },
                FilePickerFileTypes.All
            },
            AllowMultiple = false,
#pragma warning disable VSTHRD002
            SuggestedStartLocation =
                storageProvider.TryGetFolderFromPathAsync(RuntimeBasicInfo.AncillaryInformationPath).Result
#pragma warning restore VSTHRD002
        };

        var res = storageProvider.OpenFilePickerAsync(pickerOpenOptions);
        button.CommandParameter = Task.Run(async () =>
#pragma warning disable VSTHRD002
#pragma warning disable VSTHRD003
            (await res).FirstOrDefault()?.TryGetLocalPath() ?? string.Empty);
#pragma warning restore VSTHRD003
#pragma warning restore VSTHRD002
    }

    private void CreateAncillaryInfoFileCommand(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button) return;
        var storageProvider = GetTopLevel(this)!.StorageProvider;
        FilePickerSaveOptions pickerOpenOptions = new()
        {
            Title = button.Content as string,
            DefaultExtension = ".json",
            FileTypeChoices = new List<FilePickerFileType>
            {
                new("附属信息文件") { Patterns = ["*.json"] },
                FilePickerFileTypes.All
            },
#pragma warning disable VSTHRD002
            SuggestedStartLocation =
                storageProvider.TryGetFolderFromPathAsync(RuntimeBasicInfo.AncillaryInformationPath).Result
#pragma warning restore VSTHRD002
        };

        var res = storageProvider.SaveFilePickerAsync(pickerOpenOptions);
        button.CommandParameter = Task.Run(() =>
#pragma warning disable VSTHRD002
#pragma warning disable VSTHRD103
            res.Result is not null ? res.Result.TryGetLocalPath() : string.Empty);
#pragma warning restore VSTHRD103
#pragma warning restore VSTHRD002
    }

    private void ChangeSelectedRequestParamsVM(object? sender, TappedEventArgs e)
    {
        if (sender is not TextBlock textBlock) return;
        if (ViewModel?.LSPClientViewModel is null) return;
        ViewModel.LSPClientViewModel.SelectedRequestParams = textBlock.DataContext as RequestParamsViewModelBase;
    }

#pragma warning disable VSTHRD100
    private async void SelectLogFile(object? sender, RoutedEventArgs e)
#pragma warning restore VSTHRD100
    {
        if (sender is not Button button) return;
        FilePickerOpenOptions pickerOpenOptions = new()
        {
            Title = "选择日志文件",
            FileTypeFilter = new List<FilePickerFileType>
            {
                new("日志文件") { Patterns = ["*.txt", "*.log"] },
                FilePickerFileTypes.All
            },
            AllowMultiple = false
        };
        var res = await GetTopLevel(this)!.StorageProvider.OpenFilePickerAsync(pickerOpenOptions);
        if (res.FirstOrDefault()?.TryGetLocalPath() is not { } localPath) return;
        if (ViewModel?.LSPClientViewModel is null) return;
        ViewModel.LSPClientViewModel.LSPSeverLogFilePath = localPath;
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        ViewModel?.LSPClientViewModel?.Dispose();
        ViewModel = null;
    }

    private void Control_OnLoaded(object? sender, EventArgs e)
    {
        if (sender is not ReactiveUserControl<TVEFileItem> reactiveUserControl) return;
        reactiveUserControl.WhenActivated(disposable =>
        {
            reactiveUserControl.Events().DoubleTapped
                .Select(x => (x.Source as Control)?.DataContext as TVEFileItem)
                .Where(x => x != null)
                .InvokeCommand(this, x => x.ViewModel!.LSPClientViewModel!.OpenTextFileCommand!)
                .DisposeWith(disposable);
        });
    }

    private string? _lastLanguageId;

    private async void AvaloniaObject_OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (sender is not TextEditor textEditor) return;
        if (e.Property != TagProperty) return;
        if (e.NewValue is not TVEFileItem fileItem) return;
        await Task.Delay(TimeSpan.FromSeconds(1));
        try
        {
            var newLanguageId = _registryOptions.GetLanguageByExtension(Path.GetExtension(fileItem.Path)).Id;
            if (_lastLanguageId == newLanguageId) return;
            _lastLanguageId = newLanguageId;
            _textMateInstallation.SetGrammar(_registryOptions.GetScopeByLanguageId(_lastLanguageId));
        }
        catch
        {
            _textMateInstallation.SetGrammar(_lastLanguageId = null);
        }
    }

    private void TabStrip_OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property != TabStrip.SelectedIndexProperty) return;
        if (_transitioningContentControl is null) return;
        _transitioningContentControl.IsTransitionReversed = (int)e.OldValue! > (int)e.NewValue!;
    }

    private void _tabStrip_OnActualThemeVariantChanged(object? sender, EventArgs e)
    {
        var themName = (sender as Control)?.ActualThemeVariant?.Key as string;
        switch (themName)
        {
            case "Dark":
                _textMateInstallation.SetTheme(_registryOptions.LoadTheme(ThemeName.DarkPlus));
                break;
            case "Light":
                _textMateInstallation.SetTheme(_registryOptions.LoadTheme(ThemeName.LightPlus));
                break;
        }
    }

#pragma warning disable VSTHRD100
    private async void LaunchFilePicker(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button) return;
        FilePickerOpenOptions pickerOpenOptions = new()
        {
            Title = "选择LSP服务器的可执行文件",
            FileTypeFilter = new List<FilePickerFileType>
            {
                new("可执行文件") { Patterns = ["*.exe"] },
                FilePickerFileTypes.All
            },
            AllowMultiple = false
        };
        var res = await GetTopLevel(this)!.StorageProvider.OpenFilePickerAsync(pickerOpenOptions);
        if (res.Count == 0) return;
        if (res[0].TryGetLocalPath() is not { } localPath) return;
        button.Tag = localPath;
    }

    private async void LaunchFolderPicker(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button) return;
        FolderPickerOpenOptions pickerOpenOptions = new()
        {
            Title = "选择文件夹",
            AllowMultiple = false
        };
        var res = await GetTopLevel(this)!.StorageProvider.OpenFolderPickerAsync(pickerOpenOptions);
        if (res.Count == 0) return;
        if (res[0].TryGetLocalPath() is not { } localPath) return;
        button.Tag = localPath;
    }
#pragma warning restore VSTHRD100
}

public sealed class GetIconForTVEItemBase : IValueConverter
{
    public static GetIconForTVEItemBase Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            TVEFolderItem => MaterialIconKind.Folder,
            TVEFileItem => MaterialIconKind.Files,
            _ => MaterialIconKind.Null
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public sealed class TextToDocumentHB : IValueConverter
{
    public static TextToDocumentHB Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is not string text ? new TextDocument() : new TextDocument(text);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is TextDocument document ? document.Text : string.Empty;
    }
}