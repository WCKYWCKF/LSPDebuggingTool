using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using LSPDebuggingTool.Models;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace LSPDebuggingTool.ViewModels;

public class StringUI : ViewModelBase
{
    public string? Text
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}

/// Represents the view model for an LSP (Language Server Protocol) client within an application.
/// This class manages the state and behavior related to interacting with an LSP client,
/// including file operations, arguments for the language server, and workspace configurations.
/// /
public partial class LSPClientViewModel : ViewModelBase, IDisposable
{
    protected readonly CompositeDisposable _disposable = new();
    private LanguageClient? _languageClient;
    [Reactive] private bool _lSPServerIsRunning;

    private Process? _lSPServerProcess;
    [Reactive] private int _selectedIndex;

    public LSPClientViewModel()
    {
        Arguments = new ObservableCollection<StringUI>();
        _lSPServerIsRunning = false;

        _canStartLSPServer = this.WhenAnyValue(x => x.FileName, x => x.WorkspaceFolder, x => x.RootPath,
                x => x.LSPServerIsRunning)
            .Select(x =>
            {
                if (LSPServerIsRunning) return false;
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
                    new RequestGroupViewModel()
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
            .Sort(SortExpressionComparer<RequestTaskViewModelBase>.Descending(y => y.TaskStartTime))
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
        OpenedTexts = texts;
        share2.MergeMany(x => x.CloseCommand)
            .Subscribe(x => _openedTexts.Remove(x));
        share2.Connect();

        _canSendRequestAsync = this.WhenAnyValue(
                x => x.SelectedRequestParams!.ValidationContext.IsValid,
                x => x.LSPServerIsRunning)
            .Select(x => x is { Item1: true, Item2: true });

        LocationInfo = new()
        {
            OpenedTexts = OpenedTexts
        };
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
        var pipeReader = PipeReader.Create(_lSPServerProcess.StandardOutput.BaseStream);
        var pipeWriter = PipeWriter.Create(_lSPServerProcess.StandardInput.BaseStream);
        _languageClient = await Task.Run(() => LanguageClient.Create(options =>
        {
            options
                .WithInput(pipeReader)
                .WithOutput(pipeWriter)
                .WithWorkspaceFolder(DocumentUri.FromFileSystemPath(WorkspaceFolder!),
                    Path.GetDirectoryName(WorkspaceFolder!)!)
                .WithRootPath(RootPath!);
        }));
        // _languageClient = LanguageClient.Create(options =>
        // {
        //     options
        //         .WithInput(pipeReader)
        //         .WithOutput(pipeWriter)
        //         .WithWorkspaceFolder(DocumentUri.FromFileSystemPath(WorkspaceFolder!),
        //             Path.GetDirectoryName(WorkspaceFolder!)!)
        //         .WithRootPath(RootPath!);
        // });
        await _languageClient.Initialize(CancellationToken.None);
        // _languageClient.RequestSemanticTokensFull();
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

    public void Dispose()
    {
        CloseLSPServer();
        _disposable.Dispose();
    }
}

[JsonSourceGenerationOptions(WriteIndented = true, IgnoreReadOnlyFields = true, IgnoreReadOnlyProperties = true)]
[JsonSerializable(typeof(LSPClientViewModel))]
public partial class LSPClientViewModelJSC : JsonSerializerContext;