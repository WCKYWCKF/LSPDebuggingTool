using System.Text.Json.Serialization;
using EmmyLua.LanguageServer.Framework.Protocol.Model;

namespace WCKYWCKF.EmmyLua.LanguageServer.Framework.ClientEx;

public record PrepareRenameResult
{
    [JsonPropertyName("range")] public required DocumentRange Range { get; init; }
    [JsonPropertyName("placeholder")] public string? Placeholder { get; init; }
    [JsonPropertyName("defaultBehavior")] public bool DefaultBehavior { get; init; }
}