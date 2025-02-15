using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using AvaloniaEdit.Document;
using AvaloniaEdit.Highlighting;
using AvaloniaEditLSPIntegration;
using LSPDebuggingTool.Models;
using LSPDebuggingTool.ViewModels;
using Material.Icons;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using Ursa.ReactiveUIExtension;
using TextDocument = AvaloniaEdit.Document.TextDocument;

namespace LSPDebuggingTool.Views;

public partial class MainWindow : ReactiveUrsaWindow<MainWindowViewModel>
{
    private HighlightingColorizer? _highlightingColorizer;

    private string? _lastLanguageId;

    public MainWindow()
    {
        InitializeComponent();

        _lSPIntegratedTextEditor.GetSemanticColor = GetSemanticColor;

        this.WhenActivated(disposable =>
        {
            ((LSPTextEditor)_lSPIntegratedTextEditor).Events().PropertyChanged
                .Where(x => x.Property == LSPTextEditor.DocumentProperty)
                .Select(x => (x.OldValue as TextDocument, x.NewValue as TextDocument))
                .Do(async x =>
                {
                    x.Item1.Changing -= InitSemanticToekns;
                    x.Item1.Changed -= UpdateSemanticTokensAndCodeFolding;
                    x.Item2.Changing += InitSemanticToekns;
                    x.Item2.Changed += UpdateSemanticTokensAndCodeFolding;
                    if (ViewModel?.LSPClientViewModel is { } lspClientViewModel)
                    {
                        _lSPIntegratedTextEditor.LSPSemanticHighlightingEngine.InitSemanticTokens(
                            await Task.Run(() => lspClientViewModel.SemanticTokensFull()), x.Item2.Version);
                        var foldingRange = await Task.Run(() => lspClientViewModel.FoldingRanges());
                        _lSPIntegratedTextEditor.LspTextFoldingProvider.UpdateCodeFolding(foldingRange
                            .Select(x => new FoldingRange((int)x.StartLine+1, (int)x.EndLine+1)).ToList());
                    }
                })
                .Subscribe()
                .DisposeWith(disposable);
        });
    }

    private void InitSemanticToekns(object? sender, DocumentChangeEventArgs e)
    {
        if (ViewModel?.LSPClientViewModel is { } lspClientViewModel)
            lspClientViewModel.DocumentContentChanged(e);
    }

    private async void UpdateSemanticTokensAndCodeFolding(object? sender, DocumentChangeEventArgs e)
    {
        if (ViewModel?.LSPClientViewModel is { } lspClientViewModel)
        {
            var result = await Task.Run(() => lspClientViewModel.SemanticTokensDelta());
            if (result.Item1 is not null)
                _lSPIntegratedTextEditor.LSPSemanticHighlightingEngine.UpdateSemanticTokens(result.Item1,
                    _lSPIntegratedTextEditor.Document.Version);
            if (result.Item2 is not null)
                _lSPIntegratedTextEditor.LSPSemanticHighlightingEngine.UpdateSemanticTokens(result.Item2,
                    _lSPIntegratedTextEditor.Document.Version);

            var foldingRange = await Task.Run(() => lspClientViewModel.FoldingRanges());
            _lSPIntegratedTextEditor.LspTextFoldingProvider.UpdateCodeFolding(foldingRange
                .Select(x => new FoldingRange((int)x.StartLine+1, (int)x.EndLine+1)).ToList());
        }
    }


    private HighlightingColor GetSemanticColor(uint arg1, uint arg2)
    {
        return new HighlightingColor
        {
            Foreground =
                new SimpleHighlightingBrush(
                    Color.FromRgb((byte)(arg1 * 15), (byte)(arg2 * 10), (byte)((arg1 * arg2) * 5)))
        };
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

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        ViewModel?.LSPClientViewModel?.Dispose();
        ViewModel = null;
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
                break;
            case "Light":
                break;
        }
    }

    private void StyledElement_OnInitialized(object? sender, EventArgs e)
    {
        if (sender is ReactiveUserControl<TVEFileItem> control)
            control.WhenActivated(disposable =>
            {
                control.Events().DoubleTapped
                    .Select(_ => control.ViewModel)
                    .Where(x => x != null)
                    .InvokeCommand(ViewModel?.LSPClientViewModel?.OpenFileCommand!)
                    .DisposeWith(disposable);
            });
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