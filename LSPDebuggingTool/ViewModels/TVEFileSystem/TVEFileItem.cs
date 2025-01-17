using System;
using System.IO;
using AvaloniaEdit.Document;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace LSPDebuggingTool.ViewModels;

public partial class TVEFileItem : TVEItemBase
{
    public TVEFileItem(FileInfo info) : base(info, x => (x as FileInfo)?.DirectoryName ?? string.Empty)
    {
    }

    [Reactive] private bool _isSendDidOpenTextDocumentPVM;

    public TextDocument Content
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = new();

    [ReactiveCommand]
    private void Opem()
    {
        Content.Text = File.ReadAllText(Path);
    }

    [ReactiveCommand]
    private void Save()
    {
        File.WriteAllText(Path, Content.Text);
    }

    [ReactiveCommand]
    private TVEFileItem Close()
    {
        Content.Text = string.Empty;
        // IsSendDidOpenTextDocumentPVM = false;
        return this;
    }
}