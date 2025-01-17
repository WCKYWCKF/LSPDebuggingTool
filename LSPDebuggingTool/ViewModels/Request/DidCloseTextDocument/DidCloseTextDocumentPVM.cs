using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using ReactiveUI.Validation.Extensions;

namespace LSPDebuggingTool.ViewModels;

public sealed class DidCloseTextDocumentPVM : RequestParamsViewModelBase
{
    public string Tooltip { get; }

    public DidCloseTextDocumentPVM()
    {
        Title = "文档关闭通知（DidCloseTextDocument Notification）";
        Description = """
                      文档打开通知从客户端发送到服务器，以向新打开的文本文档发出信号。文档的内容现在由客户端管理，服务器不得尝试使用文档的 Uri 读取文档的内容。从这个意义上说，Open 意味着它由客户端管理。这并不一定意味着它的内容显示在编辑器中。打开通知不得多次发送，除非之前发送相应的关闭通知。这意味着打开和关闭通知必须平衡，并且特定 textDocument 的最大打开计数为 1。请注意，服务器满足请求的能力与文本文档是打开还是关闭无关。
                      """;
        Tooltip = "此请求在LSP Client工作区中打开的文件关闭时自动发送（如果你发送了文档打开通知的话）。";
        GroupName = GeneralRequestGroupNames.TextDocumentSynchronization;
    }

    public override RequestTaskViewModelBase? CreateRequestTask(LanguageClient languageClient)
    {
        return null;
    }
}