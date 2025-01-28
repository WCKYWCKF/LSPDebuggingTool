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
    public TVEFileItem(FileInfo info) : base(info, x => (x as FileInfo)?.DirectoryName ?? string.Empty)
    {
        Content.Events().TextChanged.Subscribe(_ => Version++);
    }

    [Reactive] private bool _isSendDidOpenTextDocumentPVM;
    [Reactive] private bool _isOpened;
    [Reactive] private string? _languageId;
    [Reactive] private int _version = 0;

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
            Version++;
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
        // IsSendDidOpenTextDocumentPVM = false;
        return this;
    }
}