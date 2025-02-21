namespace WCKYWCKF.EmmyLua.LanguageServer.Framework.ClientEx.Protocol;

public enum MessageType
{
    /// <summary>
    /// An error message.
    /// </summary>
    Error = 1,

    /// <summary>
    /// A warning message.
    /// </summary>
    Warning = 2,

    /// <summary>
    /// An information message.
    /// </summary>
    Info = 3,

    /// <summary>
    /// A log message.
    /// </summary>
    Log = 4,
}