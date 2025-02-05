using Avalonia;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using AvaloniaEdit.Highlighting;

namespace AvaloniaEditLSPIntegration;

/// <summary>
///     集成了LSP的<see cref="TextEditor" />（WIP）,将AvaloniaEdit的各种编辑事件抽象出来成为LSP的通知或请求。
///     <para>抽象了如下LSP通知或请求（当文档通过<see cref="ChangeDocument" />刷入<see cref="LSPTextEditor" />后在外部使用这些请求应该慎重考虑）：</para>
///     <list type="bullet">
///         <item>didChange;</item>
///     </list>
/// </summary>
public class LSPTextEditor : TextEditor
{
    public static readonly StyledProperty<bool> EnableSemanticHighlightingProperty =
        AvaloniaProperty.Register<LSPTextEditor, bool>(
            nameof(EnableSemanticHighlighting));

    public static readonly StyledProperty<bool> EnableSemanticHighlightingDeltaUpdateProperty =
        AvaloniaProperty.Register<LSPTextEditor, bool>(
            nameof(EnableSemanticHighlightingDeltaUpdate));

    public LSPTextEditor()
    {
        SemanticHighlighting = new SemanticHighlighting { Document = Document };
    }

    public SemanticHighlighting SemanticHighlighting { get; }

    public bool EnableSemanticHighlighting
    {
        get => GetValue(EnableSemanticHighlightingProperty);
        set => SetValue(EnableSemanticHighlightingProperty, value);
    }

    public bool EnableSemanticHighlightingDeltaUpdate
    {
        get => GetValue(EnableSemanticHighlightingDeltaUpdateProperty);
        set => SetValue(EnableSemanticHighlightingDeltaUpdateProperty, value);
    }

    public Action<DocumentChangeEventArgs>? DocumentContentChanged { get; set; }
    public Func<IList<int>>? SemanticTokensFull { get; set; }
    public Func<(IList<SemanticTokensEdit>?, IList<int>?)>? SemanticTokensDelta { get; set; }

    public Func<int, int, HighlightingColor>? GetSemanticColor
    {
        get => SemanticHighlighting.GetColorByTypeAndModifiers;
        set => SemanticHighlighting.GetColorByTypeAndModifiers = value;
    }

    protected override void OnDocumentChanged(DocumentChangedEventArgs e)
    {
        base.OnDocumentChanged(e);
        e.OldDocument.Changed -= OnDocumentContentChanged;
        SemanticHighlighting.Document = Document;
        Document.Changed += OnDocumentContentChanged;
        if (EnableSemanticHighlighting && SemanticTokensFull != null)
            SemanticHighlighting.InitSemanticTokens(SemanticTokensFull());
    }

    private void OnDocumentContentChanged(object? sender, DocumentChangeEventArgs e)
    {
        DocumentContentChanged?.Invoke(e);
        
        if (EnableSemanticHighlighting && SemanticTokensFull != null)
        {
            if (EnableSemanticHighlightingDeltaUpdate && SemanticTokensDelta != null)
            {
                var delta = SemanticTokensDelta();
                if (delta.Item1 != null)
                {
                    SemanticHighlighting.UpdateSemanticTokens(delta.Item1);
                }
                else if (delta.Item2 != null)
                {
                    SemanticHighlighting.UpdateSemanticTokens(delta.Item2);
                }
            }
            else
            {
                SemanticHighlighting.InitSemanticTokens(SemanticTokensFull());
            }
        }
    }

    // public void SemanticHighlightingColorChanged()
    // {
    //     for (int i = 0; i < LineCount; i++)
    //     {
    //         SemanticHighlighting.UpdateHighlightingState(i + 1);
    //     }
    // }
}