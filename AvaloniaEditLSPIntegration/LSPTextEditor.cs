using Avalonia;
using AvaloniaEdit;
using AvaloniaEdit.Highlighting;

namespace AvaloniaEditLSPIntegration;

/// <summary>
///     集成了LSP的<see cref="TextEditor" />（WIP）。提供语义高亮引擎。
/// </summary>
//todo 代码折叠，内联提示引擎，代码诊断提示引擎，输入智能补齐服务
public class LSPTextEditor : TextEditor
{
    public static readonly StyledProperty<Func<uint, uint, HighlightingColor>?> GetSemanticColorProperty =
        AvaloniaProperty.Register<LSPTextEditor, Func<uint, uint, HighlightingColor>?>(
            nameof(GetSemanticColor));

    public LSPTextEditor()
    {
        LSPSemanticHighlightingEngine = new LSPSemanticHighlightingEngine
            { Document = Document, LinesTaskExecutor = DefaultLinesTaskExecutor };
        TextArea.TextView.LineTransformers.Insert(0, new HighlightingColorizer(LSPSemanticHighlightingEngine));
    }

    public static ILinesTaskExecutor DefaultLinesTaskExecutor { get; } = ILinesTaskExecutor.Create();

    public ILinesTaskExecutor LinesTaskExecutor { get; private set; } = DefaultLinesTaskExecutor;

    protected override Type StyleKeyOverride { get; } = typeof(TextEditor);

    public LSPSemanticHighlightingEngine LSPSemanticHighlightingEngine { get; }

    public Func<uint, uint, HighlightingColor>? GetSemanticColor
    {
        get => GetValue(GetSemanticColorProperty);
        set
        {
            SetValue(GetSemanticColorProperty, value);
            LSPSemanticHighlightingEngine.GetColorByTypeAndModifiers = value;
        }
    }

    public void UseNewLinesTaskExecutor()
    {
        if (LinesTaskExecutor != DefaultLinesTaskExecutor) return;
        LinesTaskExecutor = ILinesTaskExecutor.Create();
        LSPSemanticHighlightingEngine.LinesTaskExecutor = LinesTaskExecutor;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == DocumentProperty)
            LSPSemanticHighlightingEngine.Document = Document;

        base.OnPropertyChanged(change);
    }
}