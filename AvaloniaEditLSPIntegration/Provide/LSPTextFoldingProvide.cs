using Avalonia.Threading;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using AvaloniaEdit.Folding;

namespace AvaloniaEditLSPIntegration;

public class SSS : FoldingMargin
{
    protected override void OnTextViewVisualLinesChanged()
    {
        base.OnTextViewVisualLinesChanged();
    }
}

public class LSPTextFoldingProvide
{
    public required ILinesTaskExecutor LinesTaskExecutor { get; set; }
    public FoldingManager FoldingManager { get; set; }
    public TextDocument Document { get; set; }

    public void UpdateCodeFolding(IList<FoldingRange> foldingRanges)
    {
        var list = foldingRanges.Select(x => new NewFolding(Document.GetLineByNumber(x.StartLineNumber).Offset,
                Document.GetLineByNumber(x.EndLineNumber).Offset))
            .OrderBy(x => x.StartOffset);
        Dispatcher.UIThread.Post(() => FoldingManager.UpdateFoldings(list, -1));
    }

    public LSPTextFoldingProvide(TextEditor editor)
    {
        FoldingManager = FoldingManager.Install(editor.TextArea);
    }

    ~LSPTextFoldingProvide()
    {
        FoldingManager.Uninstall(FoldingManager);
    }
}

//todo 等待更多功能的拓展
public record FoldingRange(
    int StartLineNumber,
    int EndLineNumber
);