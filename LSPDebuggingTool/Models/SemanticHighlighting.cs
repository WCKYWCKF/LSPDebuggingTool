using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Avalonia.Media;
using AvaloniaEdit.Document;
using AvaloniaEdit.Highlighting;
using LSPDebuggingTool.ViewModels;

namespace LSPDebuggingTool.Models;

public class SemanticHighlighting : IHighlighter
{
    public void Dispose()
    {
    }

    public IEnumerable<HighlightingColor> GetColorStack(int lineNumber)
    {
        return [];
    }

    public HighlightedLine HighlightLine(int lineNumber)
    {
        var line = Document.GetLineByNumber(lineNumber);
        var highlightedLine = new HighlightedLine(Document, line);
        if (line.Length != 0)
        {
            var semanticTokens = SemanticTokens
                .Where(x => x.Line == lineNumber)
                .Where(x => x.TokenType != 22);
            foreach (var semanticToken in semanticTokens)
            {
                highlightedLine.Sections.Add(new HighlightedSection()
                {
                    Offset = line.Offset + semanticToken.StartCharacter,
                    Length = semanticToken.Length,
                    Color = new HighlightingColor()
                    {
                        Background = new SimpleHighlightingBrush(Colors.Transparent),
                        Foreground = new SimpleHighlightingBrush(Color.FromRgb(
                            (byte)(semanticToken.TokenModifiers * 2),
                            (byte)semanticToken.TokenType,
                            (byte)((semanticToken.TokenModifiers + semanticToken.TokenModifiers) / 2))),
                    },
                });
            }
        }

        return highlightedLine;
    }

    public void UpdateHighlightingState(int lineNumber)
    {
    }

    public void BeginHighlighting()
    {
    }

    public void EndHighlighting()
    {
    }

    public HighlightingColor GetNamedColor(string name)
    {
        return null;
    }

    public IDocument Document { get; set; }

    public SemanticHighlighting()
    {
        SemanticTokens.CollectionChanged += (sender, args) =>
        {
            if (args.Action == NotifyCollectionChangedAction.Reset) return;
            HighlightingStateChanged?.Invoke(1, Document.LineCount);
        };
    }

    public ObservableCollection<SemanticToken> SemanticTokens { get; } = new();

    public HighlightingColor DefaultTextColor { get; } = new HighlightingColor()
    {
        Background = new SimpleHighlightingBrush(Colors.Transparent),
        Foreground = new SimpleHighlightingBrush(new Color(99, 114, 255, 0)),
        Name = nameof(DefaultTextColor)
    };

    public event HighlightingStateChangedEventHandler? HighlightingStateChanged;
}