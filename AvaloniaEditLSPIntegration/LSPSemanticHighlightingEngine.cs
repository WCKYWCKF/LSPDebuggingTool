using System.Collections.Concurrent;
using Avalonia.Threading;
using AvaloniaEdit.Document;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Utils;

namespace AvaloniaEditLSPIntegration;

public class LSPSemanticHighlightingEngine : IHighlighter
{
    private readonly ConcurrentDictionary<uint, SemanticToken[]> _flushBuffer;
    private readonly Mutex _highlightedLineFlushLock;
    private readonly Mutex _removeHighlightedLinesLock = new();

    private readonly ConcurrentDictionary<IDocumentLine, SemanticToken[]>
        _semanticTokenByLineCache = new();

    private readonly List<uint> _semanticTokens;
    private readonly CancellationTokenSource _updateSemanticHighlightingTask;
    private readonly ConcurrentQueue<Action> _updateSemanticHighlightingTaskQueue;

    private Task? _removeHighlightedLinesTask;
    private ITextSourceVersion? _semanticTokensVersion;

    public LSPSemanticHighlightingEngine()
    {
        _highlightedLineFlushLock = new Mutex();
        _semanticTokens = [];
        _flushBuffer = new ConcurrentDictionary<uint, SemanticToken[]>();
        _updateSemanticHighlightingTaskQueue = new ConcurrentQueue<Action>();
        _updateSemanticHighlightingTask = new CancellationTokenSource();
        Task.Run(() =>
        {
            // try
            // {
            while (_updateSemanticHighlightingTask.IsCancellationRequested is false)
            {
                while (_updateSemanticHighlightingTaskQueue.TryDequeue(out var task))
                {
                    task.Invoke();
                    TryFlushSemanticTokens();
                }

                Thread.Sleep(1);
            }
            // }
            // catch (Exception e)
            // {
            //     Console.WriteLine(e);
            //     throw;
            // }
        }, _updateSemanticHighlightingTask.Token);
    }

    public Func<uint, uint, HighlightingColor>? GetColorByTypeAndModifiers { get; set; }

    void IDisposable.Dispose()
    {
        _updateSemanticHighlightingTask.Cancel();
        _updateSemanticHighlightingTask.Dispose();
    }

    public IEnumerable<HighlightingColor> GetColorStack(int lineNumber)
    {
        return [];
    }

    public HighlightedLine HighlightLine(int lineNumber)
    {
        var documentLine = Document.GetLineByNumber(lineNumber);
        return _semanticTokenByLineCache.TryGetValue(documentLine, out var semanticTokens)
            ? Check()
            : new HighlightedLine(Document, documentLine);

        HighlightedLine Check()
        {
            var highlightedLine = new HighlightedLine(Document, documentLine);
            var offset = documentLine.Offset;
            highlightedLine.Sections.AddRange(semanticTokens
                .Select(x => new HighlightedSection
                {
                    Color = GetColorByTypeAndModifiers?.Invoke(x.TokenType, x.TokenModifiers) ?? DefaultTextColor,
                    Offset = (int)(offset + x.StartCharacter),
                    Length = (int)x.Length
                })
                .Where(section => section.Offset >= offset && section.Length >= 0 &&
                                  section.Offset + section.Length <= documentLine.EndOffset));
            return highlightedLine;
        }
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
        return DefaultTextColor;
    }

    public required IDocument Document
    {
        get;
        set
        {
            if (field != null)
                field.TextChanging -= FieldOnTextChanging;
            value.TextChanging += FieldOnTextChanging;

            void FieldOnTextChanging(object? sender, TextChangeEventArgs e)
            {
                _highlightedLineFlushLock.WaitOne();
                _highlightedLineFlushLock.ReleaseMutex();
            }

            field = value;
        }
    }

    public HighlightingColor DefaultTextColor { get; } = new();
    public event HighlightingStateChangedEventHandler? HighlightingStateChanged;

    public void InitSemanticTokens(IList<uint> tokens, ITextSourceVersion version)
    {
        _updateSemanticHighlightingTaskQueue.Enqueue(() =>
        {
            _semanticTokens.Clear();
            _semanticTokenByLineCache.Clear();
            UpdateSemanticTokens(tokens, version);
        });
    }

    private void TryFlushSemanticTokens()
    {
        _highlightedLineFlushLock.WaitOne();
        if (Document.Version.BelongsToSameDocumentAs(_semanticTokensVersion)
            && Document.Version.CompareAge(_semanticTokensVersion) == 0)
            foreach (var (lineNumber, semanticTokens) in _flushBuffer)
            {
                _semanticTokenByLineCache[Dispatcher.UIThread.Invoke(() => Document.GetLineByNumber((int)lineNumber))] =
                    semanticTokens;
                Dispatcher.UIThread.Post(() =>
                    HighlightingStateChanged?.Invoke((int)lineNumber, (int)lineNumber));
            }

        _highlightedLineFlushLock.ReleaseMutex();
        _flushBuffer.Clear();
    }

    public void UpdateSemanticTokens(IList<uint> deltaTokens, ITextSourceVersion version)
    {
        _updateSemanticHighlightingTaskQueue.Enqueue(() =>
        {
            var startLineNumber = _semanticTokens.Count == 0 ? 1 : GetLineNumberByOffset((int)_semanticTokens[^1]);
            _semanticTokens.AddRange(deltaTokens);
            var list = SemanticToken.Pares(deltaTokens, (uint)startLineNumber);
            foreach (var group in list.GroupBy(x => x.Line))
                UpdateSemanticTokensInLine(group);
            // 可能的性能优化方案。等待测试中
            // var latestLineNumber = list[0].Line;
            // var startLineNumberTemp = latestLineNumber;
            // var endLineNumberTemp = latestLineNumber;
            // List<(uint, uint)> events = new();
            // foreach (var u in list.Select(x => x.Line).Distinct().Skip(1).Order())
            // {
            //     if (latestLineNumber + 1 == u)
            //     {
            //         latestLineNumber = endLineNumberTemp = u;
            //     }
            //     else
            //     {
            //         events.Add((startLineNumberTemp, endLineNumberTemp));
            //         latestLineNumber = startLineNumberTemp = endLineNumberTemp = u;
            //     }
            // }
            //
            // foreach (var item in events)
            //     Dispatcher.UIThread.Post(() => HighlightingStateChanged?.Invoke((int)item.Item1, (int)item.Item2));
            RemoveHighlightedLines();
            _semanticTokensVersion = version;
        });
    }

    private void UpdateSemanticTokensInLine(IGrouping<uint, SemanticToken> group)
    {
        _flushBuffer[group.Key] = group.ToArray();
    }

    public void UpdateSemanticTokens(IList<SemanticTokensEdit> edits, ITextSourceVersion version)
    {
        _updateSemanticHighlightingTaskQueue.Enqueue(() =>
        {
            foreach (var edit in edits)
            {
                _semanticTokens.RemoveRange((int)edit.Start, (int)edit.DeleteCount);
                _semanticTokens.InsertRange((int)edit.Start, edit.Data);
                if (_semanticTokens.Count == 0) continue;
                var fullStart = Task.Run(() => GetSemanticTokensOfLineByOffset((int)edit.Start));
                var fullEnd = Task.Run(() => GetSemanticTokensOfLineByOffset((int)edit.Start + edit.Data.Count));
                //由于AvaloniaEdit的换行机制，所以多解析一行
                Task.WaitAll(fullStart, fullEnd);
                var more = GetSemanticTokensOfLineByOffset(fullEnd.Result.End.Value + 5);
                var tokens = SemanticToken.Pares(_semanticTokens[new Range(fullStart.Result.Start, more.End)],
                    (uint)GetLineNumberByOffset(fullStart.Result.Start.Value));

                foreach (var group in tokens.GroupBy(x => x.Line))
                    UpdateSemanticTokensInLine(group);
            }

            RemoveHighlightedLines();
            _semanticTokensVersion = version;
        });
    }

    private int GetLineNumberByOffset(int offset)
    {
        var line = 0;
        for (var i = 0; i < offset; i += 5) line += (int)_semanticTokens[i];

        return line + 1;
    }

    private Range GetSemanticTokensOfLineByOffset(int offset)
    {
        if (offset % 5 != 0) return Range.All;
        var startIndex = Task.Run(() => Cursor(false));
        var endIndex = Task.Run(() => Cursor(true));
        Task.WaitAll(startIndex, endIndex);
        return new Range(new Index(startIndex.Result), new Index(endIndex.Result));

        // int Cursor(bool leftOrRight)
        // {
        //     int step = leftOrRight ? 5 : -5; // 定义每次移动的步长，向左为-5，向右为5
        //
        //     // 检查边界条件和当前值是否为0
        //     if ((leftOrRight && (offset + 1 >= _semanticTokens.Count || _semanticTokens[offset] != 0)) ||
        //         (!leftOrRight && (offset - 1 < 0 || _semanticTokens[offset] != 0)))
        //     {
        //         return offset; // 如果不满足条件，直接返回当前offset
        //     }
        //     else
        //     {
        //         var index = offset;
        //         // 根据方向进行循环
        //         while ((leftOrRight && index < _semanticTokens.Count) || (!leftOrRight && index > 0))
        //         {
        //             index += step; // 根据方向移动
        //             if (index >= 0 && index < _semanticTokens.Count && _semanticTokens[index] != 0)
        //             {
        //                 break; // 如果找到非0值，退出循环
        //             }
        //         }
        //
        //         return index + (leftOrRight ? 5 : 0); // 返回最终的索引
        //     }
        // }

        //这是一串没有经过优化的代码
        int Cursor(bool leftOrRight)
        {
            if (offset % 5 != 0) return 0;
            if (offset > _semanticTokens.Count) return _semanticTokens.Count;
            if (offset == _semanticTokens.Count) return offset;
            if (leftOrRight)
            {
                if (offset + 1 >= _semanticTokens.Count || _semanticTokens[offset] != 0) return offset;

                var index = offset;
                while (index < _semanticTokens.Count && _semanticTokens[index] == 0) index += 5;

                return index;
            }

            if (offset - 1 < 0 || _semanticTokens[offset] != 0) return offset;

            {
                var index = offset;
                while (index > 0 && _semanticTokens[index] == 0) index -= 5;

                return index;
            }
        }
    }

    private void RemoveHighlightedLines()
    {
        _removeHighlightedLinesLock.WaitOne();
        _removeHighlightedLinesTask ??= Task.Run(() =>
        {
            try
            {
                foreach (var documentLine in _semanticTokenByLineCache.Keys.Where(x => x.IsDeleted))
                    _semanticTokenByLineCache.TryRemove(documentLine, out _);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            _removeHighlightedLinesLock.WaitOne();
            _removeHighlightedLinesTask = null;
            _removeHighlightedLinesLock.ReleaseMutex();
        });
        _removeHighlightedLinesLock.ReleaseMutex();
    }
}

// 以下是部分或许有效的代码设计，暂时注释留待后续观察
// public interface ITextDocumentFile
// {
//     public TextDocument Document { get; }
//     public SemanticTokensRWEngine SemanticTokensRWEngine { get; }
// }
//
// internal sealed class LinearAsynchronousExecutor
// {
//     private ConcurrentQueue<Action> _taskQueue = new();
//     private Task? _task;
//     private readonly Mutex _mutex = new();
//
//     private void Run()
//     {
//         _mutex.WaitOne();
//         _task ??= Task.Run(() =>
//         {
//             while (_taskQueue.TryDequeue(out var action))
//                 action();
//             _mutex.WaitOne();
//             _task = null;
//             _mutex.ReleaseMutex();
//         });
//         _mutex.ReleaseMutex();
//     }
//
//     public void AddTask(Action action)
//     {
//         _taskQueue.Enqueue(action);
//         Run();
//     }
// }
//
// public class SemanticTokensRWEngine
// {
//     private List<Token> _tokens;
//     private uint _cursor = 0;
//     private uint _currentLineNumber = 0;
//     private Dictionary<uint, List<SemanticToken>> _cache;
//
//     public SemanticTokensRWEngine()
//     {
//         _tokens = [];
//         _cache = [];
//     }
//
//     public void Initialize(IList<uint> tokens)
//     {
//         _tokens.Clear();
//         _tokens.AddRange(Pares(tokens));
//         _cache.Clear();
//         _cursor = 0;
//         _currentLineNumber = tokens.Count != 0 ? tokens[0] : 0;
//     }
//
//     public bool TryReadSemanticTokensByLineNumber(uint lineNumber, out List<SemanticToken> semanticTokens)
//     {
//         if (_cache.TryGetValue(lineNumber, out semanticTokens!))
//             return true;
//
//         List<uint> cache = [];
//         bool orientation = lineNumber > _currentLineNumber;
//         while (MoveCursorByToken(orientation, out var token))
//         {
//             if (_currentLineNumber == lineNumber)
//             {
//                 if (orientation)
//                     cache.AddRange([
//                         token!.DeltaLine, token.StartCharacter, token.Length, token.TokenType, token.TokenModifiers
//                     ]);
//                 else
//                     cache.InsertRange(0, [
//                         token!.DeltaLine, token.StartCharacter, token.Length, token.TokenType, token.TokenModifiers
//                     ]);
//             }
//
//             if (orientation && _currentLineNumber > lineNumber) break;
//             if (!orientation && _currentLineNumber < lineNumber) break;
//         }
//
//         semanticTokens = SemanticToken.Pares(cache, lineNumber);
//         _cache[lineNumber] = semanticTokens;
//         return semanticTokens.Count > 0;
//     }
//
//     public IEnumerable<uint> UpdateTokens(IList<uint> deltaTokens)
//     {
//         _ = SetCursor((uint)_tokens.Count - 1);
//         _tokens.AddRange(Pares(deltaTokens));
//         for (int i = 0; i < deltaTokens.Count / 5; i++)
//         {
//             MoveCursorByToken(true, out var _);
//             yield return _currentLineNumber;
//         }
//     }
//
//     // public IEnumerable<uint> UpdateTokens(IList<SemanticTokensEdit> edits)
//     // {
//     //     foreach (var edit in edits)
//     //     {
//     //         // if (edit.Start == _cursor)
//     //         // {
//     //         //     _cursor += edit.Data[0] - _tokens[(int)edit.Start];
//     //         // }
//     //         // else if (edit.Start <= _cursor)
//     //         // {
//     //         //     if (_cursor < edit.Start + edit.DeleteCount)
//     //         //     {
//     //         //         
//     //         //     }
//     //         //     else
//     //         //     {
//     //         //         
//     //         //     }
//     //         // }
//     //         if (edit.Start / 5 == 0)
//     //         {
//     //             _cursor = 0;
//     //             _currentLineNumber = edit.DeleteCount == _tokens.Count ? 0 :
//     //                 edit.Data.Count == 0 ? _currentLineNumber : edit.Data[0] + 1;
//     //         }
//     //         else if (edit.Start / 5 == _cursor)
//     //         {
//     //             // _currentLineNumber = 
//     //         }
//     //         else if (edit.Start / 5 < _cursor)
//     //             _ = SetCursor(edit.Start / 5);
//     //
//     //         _tokens.RemoveRange((int)edit.Start / 5, (int)edit.DeleteCount / 5);
//     //         _tokens.InsertRange((int)edit.Start / 5, Pares(edit.Data));
//     //
//     //         // uint GetNewLineNumber()
//     //         // {
//     //         //     if (_cursor < edit.Start / 5)
//     //         //         return _currentLineNumber;
//     //         //         
//     //         //     
//     //         //     //仅插入
//     //         //
//     //         //     //仅删除
//     //         //     //删除后插入
//     //         // }
//     //     }
//     //
//     //     _cache.Clear();
//     //     _currentLineNumber = GetLineNumberByOffset(_cursor);
//     // }
//
//     private uint GetLineNumberByOffset(uint offset)
//     {
//         uint line = 0;
//         for (var i = 0; i < offset; i += 1)
//         {
//             line += _tokens[i].DeltaLine;
//         }
//
//         return line + 1;
//     }
//
//     /// <summary>
//     /// 
//     /// </summary>
//     /// <param name="leftOrRight">false=left;true=right</param>
//     /// <param name="token"></param>
//     /// <returns></returns>
//     private bool MoveCursorByToken(bool leftOrRight, out Token? token)
//     {
//         token = null;
//         if (_currentLineNumber == 0)
//             return false;
//         checked
//         {
//             try
//             {
//                 _cursor = leftOrRight ? _cursor + 1 : _cursor - 1;
//                 token = _tokens[(int)_cursor];
//                 _currentLineNumber =
//                     leftOrRight
//                         ? _currentLineNumber + token.DeltaLine
//                         : _currentLineNumber - token.DeltaLine;
//                 return true;
//             }
//             catch (OverflowException)
//             {
//                 return false;
//             }
//         }
//     }
//
//     /// <summary>
//     /// 
//     /// </summary>
//     /// <param name="newCursor"></param>
//     /// <returns>越过了的行号</returns>
//     private IEnumerable<uint> SetCursor(uint newCursor)
//     {
//         bool leftOrRight = newCursor < _cursor;
//
//         for (uint i = 0; i < (leftOrRight ? _cursor - newCursor : newCursor - _cursor); i++)
//         {
//             MoveCursorByToken(leftOrRight, out _);
//             yield return _currentLineNumber;
//         }
//     }
//
//     private record Token(
//         uint DeltaLine,
//         uint StartCharacter,
//         uint Length,
//         uint TokenType,
//         uint TokenModifiers);
//
//     private IEnumerable<Token> Pares(IList<uint> tokens)
//     {
//         if (tokens.Count % 5 != 0)
//             throw new InvalidOperationException("Cannot paint a token that is not a multiple of 5.");
//         for (int i = 0; i < tokens.Count; i += 5)
//         {
//             yield return new Token(
//                 tokens[i],
//                 tokens[i + 1],
//                 tokens[i + 2],
//                 tokens[i + 3],
//                 tokens[i + 4]);
//         }
//     }
// }