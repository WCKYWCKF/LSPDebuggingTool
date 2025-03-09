using EmmyLua.LanguageServer.Framework;

namespace WCKYWCKF.LanguageClient;

public class LanguageClient : LSPCommunicationBase
{
    public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(5);

    public LanguageClient(Stream input, Stream output) : base(input, output)
    {
        AddJsonSerializeContext(ClientExJSC.Default);
    }
}