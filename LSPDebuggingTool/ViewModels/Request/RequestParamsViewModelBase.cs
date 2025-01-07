using System;
using System.Text.Json.Serialization;
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
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public abstract RequestTaskViewModelBase? CreateRequestTask(LanguageClient languageClient);

    [ReactiveCommand]
    private void RemoveUsageHistory() => LastUsed = DateTime.MinValue;
}