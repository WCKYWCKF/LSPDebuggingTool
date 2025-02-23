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
}