using System;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using ReactiveUI;

namespace LSPDebuggingTool.ViewModels;

public sealed class DidOpenTextDocumentTVM : RequestTaskViewModelBase<DidOpenTextDocumentParams>
{
    public override string TaskName => "文档打开通知（DidOpenTextDocument Notification）";

    public override async Task RunTaskAsync()
    {
        Start();
        try
        {
            await Task.Run(() => LanguageClient.DidOpenTextDocument(Params));
            Complete();
        }
        catch (Exception e)
        {
            Failed($"{e}");
        }
    }
}