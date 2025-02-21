namespace WCKYWCKF.EmmyLua.LanguageServer.Framework.ClientEx.Protocol;

public record LogMessageParams
{
    /// <summary>
    /// The message type. See {@link MessageType}
    /// </summary>
    public MessageType Type { get; init; }

    /// <summary>
    /// The actual message
    /// </summary>
    public required string Message { get; init; } = null!;
}