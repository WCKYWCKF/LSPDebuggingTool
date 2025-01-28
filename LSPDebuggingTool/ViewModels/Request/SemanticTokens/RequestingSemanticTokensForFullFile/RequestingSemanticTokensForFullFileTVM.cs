using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;

namespace LSPDebuggingTool.ViewModels;

public class RequestingSemanticTokensForFullFileTVM : RequestTaskViewModelBase<SemanticTokensParams>
{
    public override string TaskName { get; } = "请求完整文件语义令牌（Requesting Semantic Tokens For Full File）";

    public override async Task RunTaskAsync()
    {
        Start();
        try
        {
            CancellationTokenSource tokenSource = new();
            tokenSource.CancelAfter(TimeSpan.FromSeconds(5));
            var task = LanguageClient.RequestSemanticTokensFull(Params, tokenSource.Token);
            Result = await task;
            Complete();
        }
        catch (Exception e)
        {
            Failed($"{e}");
        }
    }

    public SemanticTokens? Result { get; private set; }
}