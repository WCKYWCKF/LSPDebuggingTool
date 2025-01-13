using System.Collections.ObjectModel;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using ReactiveUI;

namespace LSPDebuggingTool.ViewModels;

public sealed class LocationInfo : ViewModelBase
{
    // public TVEFileItem? File
    // {
    //     get;
    //     set => this.RaiseAndSetIfChanged(ref field, value);
    // }
    
    public Position? Position
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public Range? Range
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public ReadOnlyObservableCollection<TVEFileItem>? OpenedTexts
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public TVEFileItem? SelectedOpenedText
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}