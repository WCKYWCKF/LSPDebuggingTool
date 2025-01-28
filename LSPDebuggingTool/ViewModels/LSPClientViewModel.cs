using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaEdit.Document;
using DynamicData;
using DynamicData.Binding;
using LSPDebuggingTool.Models;
using LSPDebuggingTool.ViewModels.MessageBusEvent;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;
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
    private IObservable<bool> _canSendRequestAsync;
    [ObservableAsProperty] private ObservableCollectionFileSystems? _fileSystems;
    private LanguageClient? _languageClient;
    [Reactive] private bool _lSPServerIsRunning;

    private Process? _lSPServerProcess;
    [Reactive] private int _selectedIndex;
    public SemanticHighlighting SemanticHighlighting { get; } = new();

    public LSPClientViewModel()
    {
        Arguments = new ObservableCollection<StringUI>();
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

        // this.Changing.Where(x=>x.PropertyName==nameof(GeneralRequests)).Subscribe(x)
        // this.WhenAnyValue(x => x.GeneralRequests)
        //     .Select(x=>x.ToObservableChangeSet())
        //     .Switch();
        var share = this.WhenAnyValue(x => x.GeneralRequests)
            .Select(x => x.ToObservableChangeSet())
            .Switch()
            .Publish();
        share.GroupOn(x => x.GroupName)
            .Transform(x =>
            {
                var disposable = x.List.Connect()
                    .Sort(
                        SortExpressionComparer<RequestParamsViewModelBase>
                            .Descending(y => y.Title ?? string.Empty))
                    .Bind(out var collection)
                    .Subscribe();
                return new RequestGroupViewModel
                {
                    RequestsSubscribe = disposable,
                    GroupName = x.GroupKey,
                    Requests = collection
                };
            })
            .Or(share.AutoRefresh(x => x.LastUsed).GroupOn(x => x.LastUsed != DateTime.MinValue).Filter(x => x.GroupKey)
                .Transform(x =>
                    new RequestGroupViewModel
                    {
                        RequestsSubscribe = x.List.Connect()
                            .Sort(SortExpressionComparer<RequestParamsViewModelBase>.Descending(y => y.LastUsed))
                            .Bind(out var list1)
                            .Subscribe(),
                        GroupName = "使用过的请求",
                        Requests = list1
                    }))
            .Sort(SortExpressionComparer<RequestGroupViewModel>
                .Descending(x => x.GroupName == "使用过的请求")
                .ThenByAscending(x => x.GroupName))
            .Bind(out var list)
            .DisposeMany()
            .Subscribe();
        ViewGeneralRequests = list;
        share.Connect();

        RequestTasks = [];
        this.WhenAnyValue(x => x.RequestTasks)
            .Select(x => x.ToObservableChangeSet())
            .Switch()
            .Sort(SortExpressionComparer<RequestTaskViewModelBase>.Ascending(y => y.TaskStartTime))
            .Bind(out var requestTaskViewModelBases)
            .Subscribe();
        ViewRequestTasks = requestTaskViewModelBases;

        _fileSystemsHelper = this.WhenAnyValue(x => x.RootPath, x => x.LSPServerIsRunning)
            .Select(x =>
            {
                _fileSystems?.Dispose();
                return !string.IsNullOrEmpty(x.Item1) && LSPServerIsRunning
                    ? new ObservableCollectionFileSystems(x.Item1!)
                    : null;
            })
            .ToProperty(this, nameof(FileSystems));

        LogReader = new LogReader();
        LogReader.DisposeWith(_disposable);
        this.WhenAnyValue(x => x.LSPSeverLogFilePath)
            .Do(x => LogReader.Path = x)
            .Subscribe();

        var share2 = _openedTexts.ToObservableChangeSet().Publish();
        share2
            .Sort(SortExpressionComparer<TVEFileItem>.Ascending(x => x.Name))
            .Bind(out var texts)
            .Subscribe();
        ViewOpenedTexts = texts;
        share2.MergeMany(x => x.CloseCommand)
            .Subscribe(x =>
            {
                _openedTexts.Remove(x);
                if (x.IsSendDidOpenTextDocumentPVM is false || _languageClient is null) return;
                var didCloseTextDocumentTvm = new DidCloseTextDocumentTVM
                {
                    LanguageClient = _languageClient,
                    Params = new DidCloseTextDocumentParams
                        { TextDocument = new TextDocumentIdentifier(DocumentUri.File(x.Path)) }
                };
                RequestTasks.Add(didCloseTextDocumentTvm);
#pragma warning disable VSTHRD110
#pragma warning disable CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
                didCloseTextDocumentTvm.RunTaskAsync();
#pragma warning restore CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
#pragma warning restore VSTHRD110
            });
        share2.Connect();
        this.WhenAnyValue(x => x.LSPServerIsRunning)
            .Where(x => x is false)
            .Do(_ =>
            {
                _openedTexts.Clear();
                RequestTasks.Clear();
            })
            .Subscribe();

        _canSendRequestAsync = this.WhenAnyValue(
                x => x.SelectedRequestParams!.ValidationContext.IsValid,
                x => x.LSPServerIsRunning)
            .Select(x => x is { Item1: true, Item2: true });

        LocationInfo = new LocationInfo
        {
            OpenedTexts = ViewOpenedTexts
        };
        MessageBus.Current.Listen<NeedLocationInfoEvent>()
            .Subscribe(x => x.LocationInfo = LocationInfo);

        this.WhenAnyValue(x => x.SelectedOpenedText)
            .Do(x => LocationInfo.SelectedOpenedText = x)
            .Subscribe();

        //LSP Server的文档更改同步和语义令牌更新接入
        this.Events().PropertyChanging
            .Where(x => x.PropertyName == nameof(SelectedOpenedText)
                        && SelectedOpenedText is not null)
            .Do(_ =>
            {
                SelectedOpenedText!.Content.Changing -= ContentOnChanged;
                SemanticHighlighting.SemanticTokens.Clear();
            })
            .Subscribe();
        this.WhenAnyValue(x => x.SelectedOpenedText)
#pragma warning disable VSTHRD101
            .Do(async x =>
            {
                if (x is not null)
                {
                    SelectedOpenedText!.Content.Changing += ContentOnChanged;
                    SemanticHighlighting.Document = SelectedOpenedText!.Content;
                    RequestingSemanticTokensForFullFileTVM requestingSemanticTokensForFullFileTvm =
                        new()
                        {
                            LanguageClient = _languageClient!,
                            Params = new SemanticTokensParams()
                            {
                                TextDocument = new TextDocumentIdentifier(DocumentUri.File(SelectedOpenedText.Path)),
                            }
                        };
                    await SendRequestAsync(requestingSemanticTokensForFullFileTvm);
                    if (requestingSemanticTokensForFullFileTvm.Result is null) return;
                    SemanticHighlighting.SemanticTokens.AddRange(
                        SemanticToken.Pares(requestingSemanticTokensForFullFileTvm.Result));
                }
                else
                {
                    SemanticHighlighting.Document = new TextDocument();
                }
            })
#pragma warning restore VSTHRD101
            .Subscribe();
    }

    private async void ContentOnChanged(object? sender, DocumentChangeEventArgs e)
    {
        SemanticHighlighting.SemanticTokens.Clear();
        var doc = SelectedOpenedText!.Content;
        var start = doc.GetLocation(e.Offset);
        var end = doc.GetLocation(e.Offset + e.RemovalLength);
        await SendRequestAsync(new DidChangeTextDocumentTVM()
        {
            LanguageClient = _languageClient!,
            Params = new DidChangeTextDocumentParams()
            {
                TextDocument = new OptionalVersionedTextDocumentIdentifier()
                {
                    Uri = DocumentUri.File(SelectedOpenedText!.Path),
                    Version = SelectedOpenedText.Version
                },
                ContentChanges = new TextDocumentContentChangeEvent[]
                {
                    new TextDocumentContentChangeEvent()
                    {
                        Text = e.InsertedText.Text,
                        Range = new Range(start.Line - 1, start.Column - 1, end.Line - 1, end.Column - 1),
                        RangeLength = e.RemovalLength,
                    }
                }
            }
        });
        RequestingSemanticTokensForFullFileTVM requestingSemanticTokensForFullFileTvm =
            new()
            {
                LanguageClient = _languageClient!,
                Params = new SemanticTokensParams()
                {
                    TextDocument = new TextDocumentIdentifier(DocumentUri.File(SelectedOpenedText.Path)),
                }
            };
        // await _languageClient.RequestDocumentSymbol(new DocumentSymbolParams()
        // {
        //     TextDocument = new TextDocumentIdentifier(DocumentUri.File(SelectedOpenedText.Path))
        // });
        // await _languageClient!.RequestDocumentLink(new DocumentLinkParams()
        // {
        //     TextDocument = new TextDocumentIdentifier(DocumentUri.File(SelectedOpenedText.Path))
        // });
        await SendRequestAsync(requestingSemanticTokensForFullFileTvm);
        if (requestingSemanticTokensForFullFileTvm.Result is null) return;
        SemanticHighlighting.SemanticTokens.AddRange(
            SemanticToken.Pares(requestingSemanticTokensForFullFileTvm.Result));
    }

    [JsonIgnore]
    public RequestParamsViewModelBase? SelectedRequestParams
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    [JsonIgnore]
    public ObservableCollection<RequestParamsViewModelBase> GeneralRequests
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } =
    [
        new DidOpenTextDocumentPVM(),
        new DidCloseTextDocumentPVM(),
        new WillSaveTextDocumentPVM()
    ];

    [JsonIgnore] public ReadOnlyObservableCollection<RequestGroupViewModel> ViewGeneralRequests { get; }

    [JsonIgnore]
    public ObservableCollection<RequestTaskViewModelBase> RequestTasks
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    [JsonIgnore] public ReadOnlyObservableCollection<RequestTaskViewModelBase> ViewRequestTasks { get; }

    public string? LSPSeverLogFilePath
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    [JsonIgnore] public LogReader LogReader { get; }

    [JsonIgnore] public ReadOnlyObservableCollection<TVEFileItem> ViewOpenedTexts { get; }

    [JsonIgnore]
    public TVEFileItem? SelectedOpenedText
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    [JsonIgnore] public LocationInfo LocationInfo { get; }

    public void Dispose()
    {
        CloseLSPServer();
        _disposable.Dispose();
    }

    [ReactiveCommand]
    private async Task<TVEFileItem?> OpenTextFile(TVEFileItem item)
    {
        if (_openedTexts.Any(x => x.Path == item.Path)
            || _languageClient == null) return null;
        await item.OpenCommand.Execute();
        _openedTexts.Add(item);
        if (string.IsNullOrWhiteSpace(item.LanguageId) is false)
        {
            var didOpenTextDocumentTvm = new DidOpenTextDocumentTVM
            {
                LanguageClient = _languageClient,
                Params = new DidOpenTextDocumentParams
                {
                    TextDocument = new TextDocumentItem
                    {
                        LanguageId = item.LanguageId,
                        Uri = DocumentUri.File(item.Path),
                        Text = item.Content.Text,
                        Version = 0
                    }
                }
            };
            await SendRequestAsync(didOpenTextDocumentTvm);
            item.IsSendDidOpenTextDocumentPVM = true;
        }

        return item;
    }

    [ReactiveCommand(CanExecute = nameof(_canSendRequestAsync))]
    private async Task SendRequestAsync(RequestTaskViewModelBase? requestParams)
    {
        var requestTask = requestParams
                          ?? SelectedRequestParams!.CreateRequestTask(_languageClient!);
        if (requestTask is null) return;
        // SelectedRequestParams!.LastUsed = DateTime.Now;
        RequestTasks.Add(requestTask);
        await requestTask.RunTaskAsync();
    }

    [ReactiveCommand]
    private void RefreshLogReader()
    {
        LogReader.Refresh();
    }

    private StreamReader? streamReaderinput;
    private StreamReader? streamReaderoutput;

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
            },
        };
        _lSPServerProcess.StartInfo.ArgumentList.Add(Arguments.Select(x => x.Text!));
        _lSPServerProcess.Start();
        LSPServerIsRunning = true;
        streamReaderoutput = new(_lSPServerProcess.StandardOutput.BaseStream);
        _languageClient = await Task.Run(() => LanguageClient.Create(options =>
        {
            options
                .WithMaximumRequestTimeout(TimeSpan.FromSeconds(2))
                .WithInput(_lSPServerProcess.StandardOutput.BaseStream)
                .WithOutput(_lSPServerProcess.StandardInput.BaseStream)
                .WithWorkspaceFolder(DocumentUri.FromFileSystemPath(WorkspaceFolder!),
                    Path.GetDirectoryName(WorkspaceFolder!)!)
                .WithRootPath(RootPath!);
        }));
        await _languageClient.Initialize(CancellationToken.None);
    }

    [ReactiveCommand(CanExecute = nameof(_canCloseLSPServer))]
    private void CloseLSPServer()
    {
        _languageClient?.SendShutdown(new ShutdownParams());
        _languageClient?.SendExit(new ExitParams());
        _languageClient?.Dispose();
        _lSPServerProcess?.Kill(true);
        _lSPServerProcess?.Dispose();
        _lSPServerProcess = null;
        LSPServerIsRunning = false;
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