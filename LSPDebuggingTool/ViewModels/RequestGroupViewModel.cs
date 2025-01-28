using System;
using System.Collections.ObjectModel;
using ReactiveUI;

namespace LSPDebuggingTool.ViewModels;

public sealed class RequestGroupViewModel : ViewModelBase, IDisposable
{
    public required IDisposable RequestsSubscribe { get; init; }
    public required string GroupName { get; init; }
    public required ReadOnlyObservableCollection<RequestParamsViewModelBase> Requests { get; init; }

    public bool IsExpanded
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public void Dispose()
    {
        RequestsSubscribe.Dispose();
    }
}