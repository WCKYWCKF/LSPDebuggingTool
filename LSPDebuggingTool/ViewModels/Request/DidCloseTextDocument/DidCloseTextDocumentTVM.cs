using System;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace LSPDebuggingTool.ViewModels;

public sealed class DidCloseTextDocumentTVM:RequestTaskViewModelBase<DidCloseTextDocumentParams>
{
    public override string TaskName { get; } = "文档关闭通知（DidCloseTextDocument Notification）";
    public override async Task RunTaskAsync()
    {
        Start();
        try
        {
            await Task.Run(() => LanguageClient.DidCloseTextDocument(Params));
            Complete();
        }
        catch (Exception e)
        {
            Failed($"{e}");
        }
    }
}