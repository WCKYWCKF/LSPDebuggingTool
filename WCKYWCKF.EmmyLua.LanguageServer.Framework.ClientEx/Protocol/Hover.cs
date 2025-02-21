using System.Text.Json;
using System.Text.Json.Serialization;
using EmmyLua.LanguageServer.Framework.Protocol.Model.Markup;

namespace WCKYWCKF.EmmyLua.LanguageServer.Framework.ClientEx.Protocol;

public record Hover
{
    [JsonPropertyName("contents")] public required MarkedStringsOrMarkupContent Contents { get; init; }
    [JsonPropertyName("range")] public Range? Range { get; init; }
}