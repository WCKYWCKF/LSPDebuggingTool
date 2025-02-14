using Avalonia;
using AvaloniaEdit;
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
    public static readonly StyledProperty<Func<uint, uint, HighlightingColor>?> GetSemanticColorProperty =
        AvaloniaProperty.Register<LSPTextEditor, Func<uint, uint, HighlightingColor>?>(
            nameof(GetSemanticColor));

    public LSPTextEditor()
    {
        LspSemanticHighlightingEngine = new LSPSemanticHighlightingEngine { Document = Document };
        TextArea.TextView.LineTransformers.Insert(0, new HighlightingColorizer(LspSemanticHighlightingEngine));
    }

    protected override Type StyleKeyOverride { get; } = typeof(TextEditor);

    public LSPSemanticHighlightingEngine LspSemanticHighlightingEngine { get; }

    public Func<uint, uint, HighlightingColor>? GetSemanticColor
    {
        get => GetValue(GetSemanticColorProperty);
        set
        {
            SetValue(GetSemanticColorProperty, value);
            LspSemanticHighlightingEngine.GetColorByTypeAndModifiers = value;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == DocumentProperty)
            if (LspSemanticHighlightingEngine is not null)
                LspSemanticHighlightingEngine.Document = Document;
        base.OnPropertyChanged(change);
    }
}