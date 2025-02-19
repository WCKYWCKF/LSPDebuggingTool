using System.Text.Json;
using EmmyLua.LanguageServer.Framework;
using EmmyLua.LanguageServer.Framework.Protocol.JsonRpc;
using EmmyLua.LanguageServer.Framework.Protocol.Message.SemanticToken;

namespace WCKYWCKF.EmmyLua.LanguageServer.Framework.ClientEx;

public static partial class LanguageClientEx
{
    private static Task<JsonDocument?> SendRequest(
        this LSPCommunicationBase lspCommunication,
        string method,
        JsonDocument? document, TimeSpan timeOut)
    {
        using var cts = new CancellationTokenSource(timeOut);
        return lspCommunication.SendRequest(method, document, cts.Token);
    }

    private static JsonDocument SerializeToDocument(
        this LSPCommunicationBase lspCommunication,
        object @params)
    {
        return JsonSerializer.SerializeToDocument(@params, lspCommunication.JsonSerializerOptions);
    }

    private static Task<JsonDocument> SerializeToDocumentAsync(
        this LSPCommunicationBase lspCommunication,
        object @params)
    {
        return Task.Run(() => lspCommunication.SerializeToDocument(@params));
    }

    private static Task<TResult?> DeserializeAsync<TResult>(
        this LSPCommunicationBase lspCommunication,
        JsonDocument? result)
    {
        return result is null
            ? Task.FromResult(default(TResult))
            : Task.Run(() => result.Deserialize<TResult>(lspCommunication.JsonSerializerOptions));
    }

    public static async Task<SemanticTokensDeltaOrSemanticTokens?> SemanticTokensForDeltaRequest(
        this LSPCommunicationBase lspCommunication,
        SemanticTokensDeltaParams @params,
        TimeSpan timeOut)
    {
        var result = await lspCommunication.SendRequest(
            LSPDefaultMethod.textDocument_semanticTokens_full_delta,
            await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
            timeOut).ConfigureAwait(false);

        if (result is null) return null;

        try
        {
            return new SemanticTokensDeltaOrSemanticTokens(
                await lspCommunication.DeserializeAsync<SemanticTokensDelta>(result),
                null);
        }
        catch
        {
            return new SemanticTokensDeltaOrSemanticTokens(
                null,
                await lspCommunication.DeserializeAsync<SemanticTokens>(result));
        }
    }

    public static Task ShutdownRequest(
        this LSPCommunicationBase lspCommunication,
        TimeSpan timeOut)
    {
        return lspCommunication.SendRequest(LSPDefaultMethod.shutdown, null, timeOut);
    }

    public static Task ExitNotification(
        this LSPCommunicationBase lspCommunication)
    {
        return lspCommunication.SendNotification(new NotificationMessage(LSPDefaultMethod.exit, null));
    }

    // public static async Task<InitializeResult?> InitializeRequest(
    //     this LSPCommunicationBase lspCommunication,
    //     InitializeParams @params,
    //     TimeSpan timeOut)
    // {
    //     var result = await lspCommunication.SendRequest(
    //         LSPDefaultMethod.initialize,
    //         await lspCommunication.SerializeToDocumentAsync(@params),
    //         timeOut).ConfigureAwait(false);
    //     return await lspCommunication.DeserializeAsync<InitializeResult>(result).ConfigureAwait(false);
    // }
    //
    // public static async Task InitializedNotification(
    //     this LSPCommunicationBase lspCommunication,
    //     InitializeParams @params)
    // {
    //     await lspCommunication.SendNotification(
    //         new NotificationMessage(
    //             LSPDefaultMethod.initialized,
    //             await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false)));
    // }
    //
    // public static async Task DidOpenTextDocumentNotification(
    //     this LSPCommunicationBase lspCommunication,
    //     DidOpenTextDocumentParams @params)
    // {
    //     await lspCommunication.SendNotification(
    //         new NotificationMessage(
    //             LSPDefaultMethod.textDocument_didOpen,
    //             await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false)));
    // }
    //
    // public static async Task DidChangeTextDocumentNotification(
    //     this LSPCommunicationBase lspCommunication,
    //     DidChangeTextDocumentParams @params)
    // {
    //     await lspCommunication.SendNotification(
    //         new NotificationMessage(
    //             LSPDefaultMethod.textDocument_didChange,
    //             await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false)));
    // }
    //
    // public static async Task DidCloseTextDocumentNotification(
    //     this LSPCommunicationBase lspCommunication,
    //     DidCloseTextDocumentParams @params)
    // {
    //     await lspCommunication.SendNotification(
    //         new NotificationMessage(
    //             LSPDefaultMethod.textDocument_didClose,
    //             await lspCommunication.SerializeToDocumentAsync(@params))).ConfigureAwait(false);
    // }
    //
    // public static async Task<SemanticTokens?> SemanticTokensForFullRequest(
    //     this LSPCommunicationBase lspCommunication,
    //     SemanticTokensParams @params,
    //     TimeSpan timeOut)
    // {
    //     var result = await lspCommunication.SendRequest(
    //         LSPDefaultMethod.textDocument_semanticTokens_full,
    //         await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
    //         timeOut).ConfigureAwait(false);
    //     return await lspCommunication.DeserializeAsync<SemanticTokens?>(result).ConfigureAwait(false);
    // }
    //
    // public static async Task<SemanticTokensDeltaOrSemanticTokens?> SemanticTokensForDeltaRequest(
    //     this LSPCommunicationBase lspCommunication,
    //     SemanticTokensDeltaParams @params,
    //     TimeSpan timeOut)
    // {
    //     var result = await lspCommunication.SendRequest(
    //         LSPDefaultMethod.textDocument_semanticTokens_full_delta,
    //         await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
    //         timeOut).ConfigureAwait(false);
    //     if (result is null)
    //         return null;
    //     try
    //     {
    //         return new SemanticTokensDeltaOrSemanticTokens(
    //             await lspCommunication.DeserializeAsync<SemanticTokensDelta>(result).ConfigureAwait(false), null);
    //     }
    //     catch
    //     {
    //         return new SemanticTokensDeltaOrSemanticTokens(null,
    //             await lspCommunication.DeserializeAsync<SemanticTokens>(result).ConfigureAwait(false));
    //     }
    // }
    //
    // public static async Task<List<DocumentLink>?> DocumentLinkRequest(
    //     this LSPCommunicationBase lspCommunication,
    //     DocumentLinkParams @params,
    //     TimeSpan timeOut)
    // {
    //     var result = await lspCommunication.SendRequest(
    //         LSPDefaultMethod.textDocument_documentLink,
    //         await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
    //         timeOut).ConfigureAwait(false);
    //     return await lspCommunication.DeserializeAsync<List<DocumentLink>>(result).ConfigureAwait(false);
    // }
    //
    // public static async Task<List<DocumentSymbol>?> DocumentSymbolRequest(
    //     this LSPCommunicationBase lspCommunication,
    //     DocumentSymbolParams @params,
    //     TimeSpan timeOut)
    // {
    //     var result = await lspCommunication.SendRequest(
    //         LSPDefaultMethod.textDocument_documentSymbol,
    //         await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
    //         timeOut).ConfigureAwait(false);
    //     return await lspCommunication.DeserializeAsync<List<DocumentSymbol>>(result).ConfigureAwait(false);
    // }
    //
    // public static Task ShutdownRequest(
    //     this LSPCommunicationBase lspCommunication,
    //     TimeSpan timeOut)
    // {
    //     return lspCommunication.SendRequest(LSPDefaultMethod.shutdown, null, timeOut);
    // }
    //
    // public static Task ExitNotification(
    //     this global::EmmyLua.LanguageServer.Framework.Server.LanguageServer languageServer)
    // {
    //     return languageServer.SendNotification(new NotificationMessage(LSPDefaultMethod.exit, null));
    // }
    //
    // public static async Task<List<FoldingRange>?> FoldingRangeRequest(
    //     this LSPCommunicationBase lspCommunication,
    //     FoldingRangeParams @params,
    //     TimeSpan timeOut)
    // {
    //     var result = await lspCommunication.SendRequest(
    //         LSPDefaultMethod.textDocument_foldingRange,
    //         await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
    //         new CancellationTokenSource(timeOut).Token).ConfigureAwait(false);
    //     return await lspCommunication.DeserializeAsync<List<FoldingRange>>(result).ConfigureAwait(false);
    // }
}