using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using ReactiveUI;

namespace LSPDebuggingTool.ViewModels;

public class StringUI : ViewModelBase
{
    public string? Text
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}