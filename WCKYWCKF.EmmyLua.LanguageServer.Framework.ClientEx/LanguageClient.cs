using EmmyLua.LanguageServer.Framework;

namespace WCKYWCKF.EmmyLua.LanguageServer.Framework.ClientEx;

public class LanguageClient(Stream input, Stream output) : LSPCommunicationBase(input, output)
{
    public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(5);
}