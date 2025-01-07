using System;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using ReactiveUI;

namespace LSPDebuggingTool.ViewModels;

public class DidOpenTextDocumentTVM : RequestTaskViewModelBase
{
    public DidOpenTextDocumentParams? Params
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public DidOpenTextDocumentTVM()
    {
        TaskName = "文档打开通知（DidOpenTextDocument Notification）";
    }

    public override async Task RunTaskAsync()
    {
        if (LanguageClient == null || Params == null) return;
        RequestTaskStatus = RequestTaskStatus.Running;
        try
        {
            await Task.Run(() => LanguageClient.DidOpenTextDocument(Params));
            RequestTaskStatus = RequestTaskStatus.Completed;
        }
        catch (Exception e)
        {
            RequestTaskStatus = RequestTaskStatus.Failed;
            ErrorText = e.Message;
        }
    }
}