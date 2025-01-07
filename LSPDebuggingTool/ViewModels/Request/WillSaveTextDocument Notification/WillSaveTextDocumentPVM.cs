using System;
using OmniSharp.Extensions.LanguageServer.Client;

namespace LSPDebuggingTool.ViewModels;

public class WillSaveTextDocumentPVM : RequestParamsViewModelBase
{
    public WillSaveTextDocumentPVM()
    {
        Title = "将保存文本文档通知（WillSaveTextDocument Notification）";
        Description = """
                      文档保存通知是在文档实际保存之前由客户端发送到服务器的。如果服务器已注册打开/关闭事件，客户端应确保在发送将保存通知之前文档是打开的，因为客户端不能在没有所有权转让的情况下更改文件的内容。
                      """;
        GroupName = GeneralRequestGroupNames.TextDocumentSynchronization;
    }

    public override RequestTaskViewModelBase? CreateRequestTask(LanguageClient languageClient)
    {
        return null;
    }
}