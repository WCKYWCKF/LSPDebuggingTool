using System.Collections.ObjectModel;
using ReactiveUI;

namespace LSPDebuggingTool.ViewModels;

public sealed class LocationInfo : ViewModelBase
{
    // public TVEFileItem? File
    // {
    //     get;
    //     set => this.RaiseAndSetIfChanged(ref field, value);
    // }

    public int LineNumber
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public int ColumnNumber
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public ReadOnlyObservableCollection<TVEFileItem>? OpenedTexts
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}