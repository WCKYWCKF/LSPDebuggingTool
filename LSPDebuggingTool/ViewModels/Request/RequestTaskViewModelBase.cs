using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using ReactiveUI;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace LSPDebuggingTool.ViewModels;

public abstract class RequestTaskViewModelBase : ViewModelBase
{
    public abstract string TaskName { get; }

    public DateTime TaskStartTime
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public DateTime TaskEndTime
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public TimeSpan TaskDuration
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public RequestTaskStatus RequestTaskStatus
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public string? ErrorText
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    }

    [JsonIgnore] public required LanguageClient LanguageClient { get; init; }

    public int ProgressValue
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = 0;

    protected void Start()
    {
        RequestTaskStatus = RequestTaskStatus.Running;
        TaskStartTime = DateTime.Now;
    }

    protected void End(bool isFailed)
    {
        RequestTaskStatus = isFailed ? RequestTaskStatus.Failed : RequestTaskStatus.Completed;
        ProgressValue = 100;
        TaskEndTime = DateTime.Now;
        TaskDuration = TaskStartTime - TaskEndTime;
    }

    protected void Complete()
    {
        End(false);
    }

    protected void Failed(string errorText)
    {
        End(true);
        ErrorText = errorText;
    }

    public abstract Task RunTaskAsync();
}

public abstract class RequestTaskViewModelBase<T> : RequestTaskViewModelBase where T : IRequest
{
    public required T Params { get; init; }
}

public enum RequestTaskStatus
{
    Pending,
    Running,
    Completed,
    Failed
}