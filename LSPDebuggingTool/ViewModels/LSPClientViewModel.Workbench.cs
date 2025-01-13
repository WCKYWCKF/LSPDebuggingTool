using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LSPDebuggingTool.Models;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace LSPDebuggingTool.ViewModels;

public partial class LSPClientViewModel
{
    private IObservable<bool> _canSendRequestAsync;

    [JsonIgnore]
    public RequestParamsViewModelBase? SelectedRequestParams
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
    
    public ObservableCollection<RequestParamsViewModelBase> GeneralRequests
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } =
    [
        new DidOpenTextDocumentPVM(),
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

    [JsonIgnore] public LocationInfo LocationInfo { get; }

    [ReactiveCommand(CanExecute = nameof(_canSendRequestAsync))]
    private async Task SendRequestAsync()
    {
        var requestTask = SelectedRequestParams!.CreateRequestTask(_languageClient!);
        if (requestTask is null) return;
        SelectedRequestParams!.LastUsed = DateTime.Now;
        RequestTasks.Add(requestTask);
        await requestTask.RunTaskAsync();
    }
}

public sealed class RequestGroupViewModel : IDisposable
{
    public required IDisposable RequestsSubscribe { get; init; }
    public required string GroupName { get; init; }
    public required ReadOnlyObservableCollection<RequestParamsViewModelBase> Requests { get; init; }

    public void Dispose()
    {
        RequestsSubscribe.Dispose();
    }
}