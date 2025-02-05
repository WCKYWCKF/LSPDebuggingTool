using AvaloniaEdit.Document;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Utils;

namespace AvaloniaEditLSPIntegration;

public class SemanticHighlighting : IHighlighter
{
    private readonly Dictionary<IDocumentLine, HighlightedLine> _highlightedLineDictionary = new();

    private readonly List<int> _semanticTokens = new();

    public Func<int, int, HighlightingColor>? GetColorByTypeAndModifiers { get; set; }

    void IDisposable.Dispose()
    {
    }

    public IEnumerable<HighlightingColor> GetColorStack(int lineNumber)
    {
        return [];
    }

    public HighlightedLine HighlightLine(int lineNumber)
    {
        return _highlightedLineDictionary.TryGetValue(Document.GetLineByNumber(lineNumber), out var highlightedLine)
            ? highlightedLine
            : new HighlightedLine(Document, Document!.GetLineByNumber(lineNumber));
    }


    public void UpdateHighlightingState(int lineNumber)
    {
        HighlightingStateChanged?.Invoke(lineNumber, lineNumber);
    }

    public void BeginHighlighting()
    {
    }

    public void EndHighlighting()
    {
    }

    public HighlightingColor GetNamedColor(string name)
    {
        return DefaultTextColor;
    }

    public required IDocument Document
    {
        get;
        set
        {
            _semanticTokens.Clear();
            _highlightedLineDictionary.Clear();
            field = value;
        }
    }

    public HighlightingColor DefaultTextColor { get; } = new();
    public event HighlightingStateChangedEventHandler? HighlightingStateChanged;

    public void InitSemanticTokens(IList<int> tokens)
    {
        _semanticTokens.Clear();
        _highlightedLineDictionary.Clear();
        UpdateSemanticTokens(tokens);
    }

    public void UpdateSemanticTokens(IList<int> deltaTokens)
    {
        _semanticTokens.AddRange(deltaTokens);
        var startLineNumber = _semanticTokens.Count == 0 ? 1 : GetLineNumberByOffset(_semanticTokens[^1]);
        var list = SemanticToken.Pares(deltaTokens, startLineNumber);
        foreach (var group in list.GroupBy(x => x.Line))
            UpdateSemanticTokensInLine(group);
        RemoveHighlightedLines();
    }

    private void UpdateSemanticTokensInLine(IGrouping<int, SemanticToken> group)
    {
        var documentLine = Document.GetLineByNumber(group.Key);
        if (_highlightedLineDictionary.TryGetValue(documentLine, out var highlightedLine) is false)
            _highlightedLineDictionary[documentLine] = highlightedLine = new HighlightedLine(Document, documentLine);
        var offset = highlightedLine.DocumentLine.Offset;
        highlightedLine.Sections.Clear();
        highlightedLine.Sections.AddRange(group.Select(x => new HighlightedSection
        {
            Color = GetColorByTypeAndModifiers?.Invoke(x.TokenType, x.TokenModifiers) ?? DefaultTextColor,
            Offset = offset + x.StartCharacter,
            Length = x.Length
        }));
        try
        {
            highlightedLine.ValidateInvariants();
        }
        catch (InvalidOperationException)
        {
            highlightedLine.Sections.Clear();
        }

        UpdateHighlightingState(group.Key);
    }

    public void UpdateSemanticTokens(IList<SemanticTokensEdit> edits)
    {
        foreach (var edit in edits)
        {
            _semanticTokens.RemoveRange(edit.Start, edit.DeleteCount);
            _semanticTokens.AddRange(edit.Data);
            var tokens = SemanticToken.Pares(edit.Data, edit.Start);
            foreach (var group in tokens.GroupBy(x => x.Line))
                UpdateSemanticTokensInLine(group);
        }

        RemoveHighlightedLines();
    }

    private int GetLineNumberByOffset(int offset)
    {
        var line = 0;
        for (var i = 0; i < _semanticTokens.Count; i += 5)
        {
            if (offset < _semanticTokens[i + 1])
                return line;
            line = _semanticTokens[i];
        }

        return line;
    }

    private void RemoveHighlightedLines()
    {
        foreach (var documentLine in _highlightedLineDictionary.Keys.Where(x => x.IsDeleted))
            _highlightedLineDictionary.Remove(documentLine);
    }
}