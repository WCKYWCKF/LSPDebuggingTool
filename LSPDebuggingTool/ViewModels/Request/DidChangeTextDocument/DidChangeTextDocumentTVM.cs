using System;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace LSPDebuggingTool.ViewModels;

public class DidChangeTextDocumentTVM : RequestTaskViewModelBase<DidChangeTextDocumentParams>
{
    public override string TaskName { get; } = "文档更改通知（DidChangeTextDocument Notification）";

    public override async Task RunTaskAsync()
    {
        Start();
        try
        {
            await Task.Run(() => LanguageClient.DidChangeTextDocument(Params));
            Complete();
        }
        catch (Exception e)
        {
            Failed($"{e}");
        }
    }
}