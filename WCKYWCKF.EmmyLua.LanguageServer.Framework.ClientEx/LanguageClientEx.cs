using System.Text.Json;
using EmmyLua.LanguageServer.Framework.Protocol.JsonRpc;
using EmmyLua.LanguageServer.Framework.Protocol.Message.DocumentLink;
using EmmyLua.LanguageServer.Framework.Protocol.Message.DocumentSymbol;
using EmmyLua.LanguageServer.Framework.Protocol.Message.Initialize;
using EmmyLua.LanguageServer.Framework.Protocol.Message.SemanticToken;
using EmmyLua.LanguageServer.Framework.Protocol.Message.TextDocument;

namespace WCKYWCKF.EmmyLua.LanguageServer.Framework.ClientEx;

public static class LanguageClientEx
{
    public static JsonDocument SerializeToDocument(
        this global::EmmyLua.LanguageServer.Framework.Server.LanguageServer languageServer,
        object @params)
    {
        return JsonSerializer.SerializeToDocument(@params, languageServer.JsonSerializerOptions);
    }

    public static TResult? Deserialize<TResult>(
        this global::EmmyLua.LanguageServer.Framework.Server.LanguageServer languageServer,
        JsonDocument result)
    {
        return result.Deserialize<TResult>(languageServer.JsonSerializerOptions);
    }

    public static async Task<InitializeResult?> SendInitializeRequest(
        this global::EmmyLua.LanguageServer.Framework.Server.LanguageServer languageServer,
        InitializeParams @params,
        TimeSpan timeOut)
    {
        var resultJson = await languageServer.SendRequest(
            "initialize",
            languageServer.SerializeToDocument(@params),
            new CancellationTokenSource(timeOut).Token).ConfigureAwait(false);

        if (resultJson is null)
            throw new NullReferenceException(nameof(resultJson));
        return languageServer.Deserialize<InitializeResult>(resultJson);
    }

    public static Task SendInitializedNotification(
        this global::EmmyLua.LanguageServer.Framework.Server.LanguageServer languageServer)
    {
        return languageServer.SendNotification(
            new NotificationMessage(
                "initialized",
                languageServer.SerializeToDocument(new InitializedParams())));
    }

    public static Task SendDidOpenTextDocumentNotification(
        this global::EmmyLua.LanguageServer.Framework.Server.LanguageServer languageServer,
        DidOpenTextDocumentParams @params)
    {
        return languageServer.SendNotification(
            new NotificationMessage(
                "textDocument/didOpen",
                languageServer.SerializeToDocument(@params)));
    }

    public static Task SendDidChangeTextDocumentNotification(
        this global::EmmyLua.LanguageServer.Framework.Server.LanguageServer languageServer,
        DidChangeTextDocumentParams @params)
    {
        return languageServer.SendNotification(
            new NotificationMessage(
                "textDocument/didChange",
                languageServer.SerializeToDocument(@params)));
    }

    public static Task SendDidCloseTextDocumentNotification(
        this global::EmmyLua.LanguageServer.Framework.Server.LanguageServer languageServer,
        DidCloseTextDocumentParams @params)
    {
        return languageServer.SendNotification(
            new NotificationMessage(
                "textDocument/didClose",
                languageServer.SerializeToDocument(@params)));
    }

    public static async Task<SemanticTokens?> SendSemanticTokensForFullRequest(
        this global::EmmyLua.LanguageServer.Framework.Server.LanguageServer languageServer,
        SemanticTokensParams @params,
        TimeSpan timeOut)
    {
        var result = await languageServer.SendRequest(
            "textDocument/semanticTokens/full",
            languageServer.SerializeToDocument(@params),
            new CancellationTokenSource(timeOut).Token).ConfigureAwait(false);
        return result is null ? null : languageServer.Deserialize<SemanticTokens>(result);
    }

    public static async Task<object?> SendSemanticTokensForDeltaRequest(
        this global::EmmyLua.LanguageServer.Framework.Server.LanguageServer languageServer,
        SemanticTokensDeltaParams @params,
        TimeSpan timeOut)
    {
        var result = await languageServer.SendRequest(
            "textDocument/semanticTokens/full/delta",
            languageServer.SerializeToDocument(@params),
            new CancellationTokenSource(timeOut).Token).ConfigureAwait(false);
        try
        {
            return result is null ? null : languageServer.Deserialize<SemanticTokensDelta>(result);
        }
        catch
        {
            return result is null ? null : languageServer.Deserialize<SemanticTokens>(result);
        }
    }

    public static async Task<List<DocumentLink>?> SendDocumentLinkRequest(
        this global::EmmyLua.LanguageServer.Framework.Server.LanguageServer languageServer,
        DocumentLinkParams @params,
        TimeSpan timeOut)
    {
        var result = await languageServer.SendRequest(
            "textDocument/documentLink",
            languageServer.SerializeToDocument(@params),
            new CancellationTokenSource(timeOut).Token).ConfigureAwait(false);
        return result is null ? null : languageServer.Deserialize<List<DocumentLink>>(result);
    }

    public static async Task<List<DocumentSymbol>?> SendDocumentSymbolRequest(
        this global::EmmyLua.LanguageServer.Framework.Server.LanguageServer languageServer,
        DocumentSymbolParams @params,
        TimeSpan timeOut)
    {
        var result = await languageServer.SendRequest(
            "textDocument/documentSymbol",
            languageServer.SerializeToDocument(@params),
            new CancellationTokenSource(timeOut).Token).ConfigureAwait(false);
        return result is null ? null : languageServer.Deserialize<List<DocumentSymbol>>(result);
    }

    public static Task SendShutdownRequest(
        this global::EmmyLua.LanguageServer.Framework.Server.LanguageServer languageServer,
        TimeSpan timeOut)
    {
        return languageServer.SendRequest("shutdown", null, new CancellationTokenSource(timeOut).Token);
    }

    public static Task SendExitNotification(
        this global::EmmyLua.LanguageServer.Framework.Server.LanguageServer languageServer)
    {
        return languageServer.SendNotification(new NotificationMessage("exit", null));
    }
}