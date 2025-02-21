using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace WCKYWCKF.EmmyLua.LanguageServer.Framework.ClientEx.Protocol;

public record DidChangeConfigurationParams
{
    /// <summary>
    /// The actual changed settings
    /// </summary>
    ///
    [JsonPropertyName("settings")]
    public JsonObject? Settings { get; init; }
}