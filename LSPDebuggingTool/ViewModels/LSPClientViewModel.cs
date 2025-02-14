using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using AvaloniaEdit.Document;
using DynamicData;
using DynamicData.Binding;
using EmmyLua.LanguageServer.Framework.Protocol.Capabilities.Client.ClientCapabilities;
using EmmyLua.LanguageServer.Framework.Protocol.Capabilities.Client.TextDocumentClientCapabilities;
using EmmyLua.LanguageServer.Framework.Protocol.Message.Initialize;
using EmmyLua.LanguageServer.Framework.Protocol.Message.SemanticToken;
using EmmyLua.LanguageServer.Framework.Protocol.Message.TextDocument;
using EmmyLua.LanguageServer.Framework.Protocol.Model;
using EmmyLua.LanguageServer.Framework.Protocol.Model.TextDocument;
using EmmyLua.LanguageServer.Framework.Server;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using WCKYWCKF.EmmyLua.LanguageServer.Framework.ClientEx;
using SemanticTokensEdit = AvaloniaEditLSPIntegration.SemanticTokensEdit;
using TextDocument = AvaloniaEdit.Document.TextDocument;

namespace LSPDebuggingTool.ViewModels;

/// Represents the view model for an LSP (Language Server Protocol) client within an application.
/// This class manages the state and behavior related to interacting with an LSP client,
/// including file operations, arguments for the language server, and workspace configurations.
/// /
public partial class LSPClientViewModel : ViewModelBase, IDisposable
{
    protected readonly CompositeDisposable _disposable = new();

    private readonly ObservableCollection<TVEFileItem> _openedTexts = [];

    private CancellationTokenSource? _cancellationTokenSourceForLogReaderTask;

    [ObservableAsProperty] private ObservableCollectionFileSystems? _fileSystems;
    [Reactive] private bool _lSPServerIsRunning;

    private Process? _lSPServerProcess;
    [Reactive] private int _selectedIndex;
    private PipeWriter? _serverInputWriter;

    private PipeReader? _serverOutputReader;
    private StreamReader? _serverRunLogReader;
    public LanguageServer? LanguageClientForEdit;

    public LSPClientViewModel()
    {
        Arguments = new ObservableCollection<StringUI>();
        LogText = new TextDocument();
        ViewOpenedTexts = new ObservableCollection<TVEFileItem>();
        _lSPServerIsRunning = false;

        _canStartLSPServer = this.WhenAnyValue(x => x.FileName, x => x.WorkspaceFolder, x => x.RootPath,
                x => x.LSPServerIsRunning)
            .Select(x =>
            {
                if (x.Item4) return false;
                return File.Exists(x.Item1) && Directory.Exists(x.Item2) && Directory.Exists(x.Item3);
            });

        _canCloseLSPServer = this.WhenAnyValue(x => x.LSPServerIsRunning);

        _canRemoveArgument = this.WhenAnyValue(x => x.SelectedIndex)
            .Select(x => x >= 0);

        _fileSystemsHelper = this.WhenAnyValue(x => x.RootPath)
            .Select(x => string.IsNullOrWhiteSpace(x) ? null : new ObservableCollectionFileSystems(x))
            .ToProperty(this, nameof(FileSystems));

        ViewOpenedTexts.ToObservableChangeSet()
            .MergeMany(x => x.CloseCommand)
            .InvokeCommand(CloseFileCommand);

        DocumentContentChanged = DocumentContentChangedP;
        SemanticTokensFull = SemanticTokensFullP;
        SemanticTokensDelta = SemanticTokensDeltaP;
    }

    [JsonIgnore] public TextDocument LogText { get; }

    [JsonIgnore] public ObservableCollection<TVEFileItem> ViewOpenedTexts { get; }

    [JsonIgnore]
    public TVEFileItem? SelectedOpenedText
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public Func<(IList<SemanticTokensEdit>?, IList<uint>?)> SemanticTokensDelta { get; }

    public Action<DocumentChangeEventArgs> DocumentContentChanged { get; }

    public Func<IList<uint>> SemanticTokensFull { get; }

    public void Dispose()
    {
        CloseLSPServer();
        _disposable.Dispose();
    }

    [ReactiveCommand]
    private async Task OpenFileAsync(TVEFileItem item)
    {
        await item.OpenCommand.Execute();
        await LanguageClientForEdit!.SendDidOpenTextDocumentNotification(new DidOpenTextDocumentParams
        {
            TextDocument = new TextDocumentItem
            {
                Uri = new DocumentUri(new Uri(item.Path)),
                LanguageId = item.LanguageId ?? "plaintext",
                Text = item.Content.Text,
                Version = item.Version
            }
        });
        ViewOpenedTexts.Add(item);
    }

    [ReactiveCommand]
    private Task CloseFileAsync(TVEFileItem item)
    {
        ViewOpenedTexts.Remove(item);
        // await item.CloseCommand.Execute();
        return LanguageClientForEdit!.SendDidCloseTextDocumentNotification(new DidCloseTextDocumentParams
        {
            TextDocument = new TextDocumentIdentifier(new DocumentUri(new Uri(item.Path)))
        });
    }

    [ReactiveCommand(CanExecute = nameof(_canStartLSPServer))]
    private async Task StartLSPServerAsync()
    {
        _lSPServerProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = FileName,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };
        _lSPServerProcess.StartInfo.ArgumentList.Add(Arguments.Select(x => x.Text!));
        _lSPServerProcess.Start();
        LSPServerIsRunning = true;
        _serverInputWriter = PipeWriter.Create(_lSPServerProcess.StandardInput.BaseStream);
        _serverOutputReader = PipeReader.Create(_lSPServerProcess.StandardOutput.BaseStream);
        _serverRunLogReader = new StreamReader(_lSPServerProcess.StandardError.BaseStream);

        LanguageClientForEdit =
            LanguageServer.From(_lSPServerProcess.StandardOutput.BaseStream, _serverInputWriter.AsStream());
        LanguageClientForEdit.AddJsonSerializeContext(JsonProtocolContext.Default);
        Task.Run(LanguageClientForEdit.Run);
        // Task.Run(LanguageClientForEdit.Run);
        LogText.Text = string.Empty;
        _cancellationTokenSourceForLogReaderTask = new CancellationTokenSource();
        Task.Run(LogReaderTask);
        var serverC = await LanguageClientForEdit.SendInitializeRequest(new InitializeParams
        {
            ProcessId = null,
            ClientInfo = new ClientInfo
            {
                Name = "Typst LSP test",
                Version = "1.0.0"
            },
            Locale = "zh-cn",
            RootUri = new DocumentUri(new Uri(RootPath)),
            RootPath = RootPath,
            WorkspaceFolders =
            [
                new WorkspaceFolder(new DocumentUri(new Uri(WorkspaceFolder)), WorkspaceFolder)
            ],
            Trace = TraceValue.Off,
            Capabilities = new ClientCapabilities
            {
                Workspace = new WorkspaceClientCapabilities
                {
                    WorkspaceFolders = true
                },
                TextDocument = new TextDocumentClientCapabilities(),
                Window = new WindowClientCapabilities
                {
                    WorkDoneProgress = true
                },
                General = new GeneralClientCapabilities(),
                Experimental = JsonDocument.Parse("{}")
            },
            InitializationOptions = JsonDocument.Parse("{}")
        }, TimeSpan.FromSeconds(2));
        await LanguageClientForEdit.SendInitializedNotification();
    }

    private async Task LogReaderTask()
    {
        if (_cancellationTokenSourceForLogReaderTask is null) return;
        while (LSPServerIsRunning && _serverRunLogReader != null)
        {
            var result = await _serverRunLogReader.ReadLineAsync(_cancellationTokenSourceForLogReaderTask.Token);
            var str = result;
            // string str = Encoding.UTF8.GetString(result.Buffer);
            Console.WriteLine(str);
            await Dispatcher.UIThread.InvokeAsync(() => LogText.Insert(LogText.TextLength, str));
        }
    }

    private (IList<SemanticTokensEdit>?, IList<uint>?) SemanticTokensDeltaP()
    {
        if (LSPServerIsRunning
            && LanguageClientForEdit is not null
            && SelectedOpenedText is not null)
        {
            var result = LanguageClientForEdit.SendSemanticTokensForDeltaRequest(new SemanticTokensDeltaParams
            {
                TextDocument = new TextDocumentIdentifier(new DocumentUri(new Uri(SelectedOpenedText.Path))),
                PreviousResultId = SelectedOpenedText.LatestSemanticVersion!
            }, TimeSpan.FromSeconds(2)).Result;
            switch (result)
            {
                case SemanticTokensDelta semanticTokensDelta:
                    SelectedOpenedText.LatestSemanticVersion = semanticTokensDelta.ResultId;
                    return (
                        semanticTokensDelta.Edits
                            .Select(x => new SemanticTokensEdit(x.Start, x.DeleteCount, x.Data ?? []))
                            .ToList(), null);
                case SemanticTokens semanticTokens:
                    SelectedOpenedText.LatestSemanticVersion = semanticTokens.ResultId;
                    return (null, semanticTokens.Data);
            }
        }

        return (null, null);
    }

    private void DocumentContentChangedP(DocumentChangeEventArgs obj)
    {
        if (LSPServerIsRunning
            && LanguageClientForEdit is not null
            && SelectedOpenedText is not null)
        {
            var start = SelectedOpenedText.Content.GetLocation(obj.Offset);
            var end = SelectedOpenedText.Content.GetLocation(obj.Offset + obj.RemovalLength);
            LanguageClientForEdit.SendDidChangeTextDocumentNotification(new DidChangeTextDocumentParams
            {
                TextDocument = new VersionedTextDocumentIdentifier(new DocumentUri(new Uri(SelectedOpenedText.Path)),
                    SelectedOpenedText.Version),
                ContentChanges =
                [
                    new TextDocumentContentChangeEvent
                    {
                        Range = new DocumentRange(new Position(start.Line - 1, start.Column - 1),
                            new Position(end.Line - 1, end.Column - 1)),
                        RangeLength = obj.RemovalLength,
                        Text = obj.InsertedText.Text
                    }
                ]
            }).Wait();
        }
    }

    private IList<uint> SemanticTokensFullP()
    {
        if (LSPServerIsRunning
            && LanguageClientForEdit is not null
            && SelectedOpenedText is not null)
        {
            var result = Task.Run(async () => await LanguageClientForEdit.SendSemanticTokensForFullRequest(
                new SemanticTokensParams
                {
                    TextDocument = new TextDocumentIdentifier(new DocumentUri(new Uri(SelectedOpenedText.Path)))
                }, TimeSpan.FromSeconds(2))).Result;
            if (result is null) return [];
            SelectedOpenedText.LatestSemanticVersion = result.ResultId;
            return result.Data;
        }

        return [];
    }

    [ReactiveCommand(CanExecute = nameof(_canCloseLSPServer))]
    private void CloseLSPServer()
    {
        LanguageClientForEdit?.SendShutdownRequest(TimeSpan.FromSeconds(2)).Wait();
        LanguageClientForEdit?.SendExitNotification();
        LSPServerIsRunning = false;
        _cancellationTokenSourceForLogReaderTask?.Cancel();
        _cancellationTokenSourceForLogReaderTask?.Dispose();
        _lSPServerProcess?.Kill(true);
        _lSPServerProcess?.Dispose();
        _lSPServerProcess = null;
    }

    [ReactiveCommand]
    private void AddArgument()
    {
        Arguments.Add(new StringUI());
    }

    [ReactiveCommand(CanExecute = nameof(_canRemoveArgument))]
    private void RemoveArgument(StringUI argument)
    {
        Arguments.Remove(argument);
    }

    [ReactiveCommand]
    private void ArgumentEditCompleteCheck(string argument)
    {
        if (string.IsNullOrEmpty(argument)) Arguments.RemoveMany(Arguments.Where(x => string.IsNullOrEmpty(x.Text)));
    }

    #region ProcessStartInfo

    public string? FileName
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
    public ObservableCollection<StringUI> Arguments
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    #endregion

    #region LSPServerInit

    public string? WorkspaceFolder
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public string? RootPath
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    #endregion

    #region CanCommand

    private readonly IObservable<bool> _canStartLSPServer;
    private readonly IObservable<bool> _canCloseLSPServer;
    private readonly IObservable<bool> _canRemoveArgument;

    #endregion
}

[JsonSerializable(typeof(SemanticTokensDeltaParams))]
[JsonSerializable(typeof(SemanticTokensDelta))]
public partial class JsonProtocolContext : JsonSerializerContext;