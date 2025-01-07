using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using ReactiveUI;

namespace LSPDebuggingTool.ViewModels;

[JsonDerivedType(typeof(DidOpenTextDocumentTVM), nameof(DidOpenTextDocumentTVM))]
public abstract class RequestTaskViewModelBase : ViewModelBase
{
    protected RequestTaskViewModelBase()
    {
        this.WhenAnyValue(x => x.RequestTaskStatus)
            .Subscribe(x =>
            {
                switch (x)
                {
                    case RequestTaskStatus.Running:
                        TaskStartTime = DateTime.Now;
                        break;
                    case RequestTaskStatus.Completed or RequestTaskStatus.Failed:
                        TaskEndTime = DateTime.Now;
                        break;
                }
            });
    }

    public string? TaskName
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public DateTime TaskStartTime
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public DateTime TaskEndTime
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public RequestTaskStatus RequestTaskStatus
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public string? ErrorText
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    [JsonIgnore]
    public LanguageClient? LanguageClient
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    [JsonIgnore]
    public bool IsExpanded
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = false;

    public abstract Task RunTaskAsync();
}

public enum RequestTaskStatus
{
    Pending,
    Running,
    Completed,
    Failed
}