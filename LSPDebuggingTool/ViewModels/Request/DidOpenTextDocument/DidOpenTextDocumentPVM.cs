using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using LSPDebuggingTool.ViewModels.MessageBusEvent;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

namespace LSPDebuggingTool.ViewModels;

public class DidOpenTextDocumentPVM : RequestParamsViewModelBase
{
    public string? FilePath
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public string? LanguageId
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public int? Version
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public DidOpenTextDocumentPVM()
    {
        Title = "文档打开通知（DidOpenTextDocument Notification）";
        Description = """
                      文档打开通知从客户端发送到服务器，以向新打开的文本文档发出信号。文档的内容现在由客户端管理，服务器不得尝试使用文档的 Uri 读取文档的内容。从这个意义上说，Open 意味着它由客户端管理。这并不一定意味着它的内容显示在编辑器中。打开通知不得多次发送，除非之前发送相应的关闭通知。这意味着打开和关闭通知必须平衡，并且特定 textDocument 的最大打开计数为 1。请注意，服务器满足请求的能力与文本文档是打开还是关闭无关。
                      """;
        GroupName = GeneralRequestGroupNames.TextDocumentSynchronization;

        this.ValidationRule(vm => vm.FilePath, x =>
        {
            NeedLocationInfoEvent needLocationInfoEvent = new();
            MessageBus.Current.SendMessage(needLocationInfoEvent);
            var openedTexts = needLocationInfoEvent?.LocationInfo?.OpenedTexts;
            return openedTexts is not null && openedTexts.Any(y => y.Path == x);
        }, "必须选择一个已经打开的文件");
        this.ValidationRule(vm => vm.LanguageId, x => LanguageIdentifier.LanguageIds.Any(y => y.LanguageId == x),
            "这不是任何一个已知LanguageId,你需要确保它是正确的");
    }

    public override RequestTaskViewModelBase? CreateRequestTask(LanguageClient languageClient)
    {
        var openText = Location?.OpenedTexts?.FirstOrDefault(x => x.Path == FilePath);
        if (openText is null) return null;
        openText.IsSendDidOpenTextDocumentPVM = true;
        return new DidOpenTextDocumentTVM()
        {
            LanguageClient = languageClient,
            Params = new DidOpenTextDocumentParams()
            {
                TextDocument = new TextDocumentItem
                {
                    LanguageId = LanguageId!,
                    Uri = DocumentUri.File(FilePath!),
                    Text = openText.Content.Text,
                    Version = Version
                }
            }
        };
    }
}