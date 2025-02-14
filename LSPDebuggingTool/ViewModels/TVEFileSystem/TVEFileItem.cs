using System;
using System.IO;
using System.Threading.Tasks;
using AvaloniaEdit.Document;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace LSPDebuggingTool.ViewModels;

public partial class TVEFileItem : TVEItemBase
{
    public static TextDocument NullDocument = new();
    [Reactive] private bool _isOpened;

    [Reactive] private bool _isSendDidOpenTextDocumentPVM;
    [Reactive] private string? _languageId;
    [Reactive] private string? _latestSemanticVersion;
    [Reactive] private int _version;

    public TVEFileItem(FileInfo info) : base(info, x => (x as FileInfo)?.DirectoryName ?? string.Empty)
    {
        Content.Events().TextChanged.Subscribe(_ => Version++);
    }

    public TextDocument Content
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    } = new();

    [ReactiveCommand]
    private async Task OpenAsync()
    {
        try
        {
            LanguageId = GetLanguageId();
            Version = 1;
            Content.Text = await File.ReadAllTextAsync(Path);
        }
        catch (Exception e)
        {
            Content.Text = $"{e}";
        }
    }

    private string GetLanguageId()
    {
        return Path switch
        {
            var x when x.EndsWith(".c") || x.EndsWith(".h") => "c",
            var x when x.EndsWith(".typ") => "typst",
            _ => string.Empty
        };
    }

    [ReactiveCommand]
    private async Task SaveAsync()
    {
        await File.WriteAllTextAsync(Path, Content.Text);
    }

    [ReactiveCommand]
    private TVEFileItem Close()
    {
        Version = 1;
        LatestSemanticVersion = null;
        return this;
    }
}