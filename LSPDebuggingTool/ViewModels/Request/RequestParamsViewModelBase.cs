using System;
using System.Text.Json.Serialization;
using LSPDebuggingTool.ViewModels.MessageBusEvent;
using OmniSharp.Extensions.LanguageServer.Client;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using ReactiveUI.Validation.Helpers;

namespace LSPDebuggingTool.ViewModels;

[JsonDerivedType(typeof(DidOpenTextDocumentPVM), nameof(DidOpenTextDocumentPVM))]
[JsonDerivedType(typeof(WillSaveTextDocumentPVM), nameof(WillSaveTextDocumentPVM))]
public abstract partial class RequestParamsViewModelBase : ReactiveValidationObject
{
    [JsonIgnore]
    public string? Title
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    [JsonIgnore]
    public string? Description
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public DateTime LastUsed
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    [JsonIgnore]
    public string GroupName
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "未设置";

    [JsonIgnore]
    public LocationInfo? Location
    {
        get
        {
            NeedLocationInfoEvent needLocationInfoEvent = new();
            MessageBus.Current.SendMessage(needLocationInfoEvent);
            return needLocationInfoEvent.LocationInfo;
        }
    }

    public abstract RequestTaskViewModelBase? CreateRequestTask(LanguageClient languageClient);

    [ReactiveCommand]
    private void RemoveUsageHistory() => LastUsed = DateTime.MinValue;
}