using AvaloniaEdit.Document;
using AvaloniaEdit.Highlighting;

namespace AvaloniaEditLSPIntegration;

public class SemanticHighlighting:IHighlighter
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<HighlightingColor> GetColorStack(int lineNumber)
    {
        throw new NotImplementedException();
    }

    public HighlightedLine HighlightLine(int lineNumber)
    {
        throw new NotImplementedException();
    }

    public void UpdateHighlightingState(int lineNumber)
    {
        throw new NotImplementedException();
    }

    public void BeginHighlighting()
    {
        throw new NotImplementedException();
    }

    public void EndHighlighting()
    {
        throw new NotImplementedException();
    }

    public HighlightingColor GetNamedColor(string name)
    {
        throw new NotImplementedException();
    }

    public IDocument Document { get; }
    public HighlightingColor DefaultTextColor { get; }
    public event HighlightingStateChangedEventHandler? HighlightingStateChanged;
}