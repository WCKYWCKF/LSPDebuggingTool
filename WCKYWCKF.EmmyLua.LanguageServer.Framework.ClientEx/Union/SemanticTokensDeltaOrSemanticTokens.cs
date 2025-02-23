using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using EmmyLua.LanguageServer.Framework.Protocol.Message.SemanticToken;

namespace WCKYWCKF.EmmyLua.LanguageServer.Framework.ClientEx.Union;

[JsonConverter(typeof(SemanticTokensDeltaOrSemanticTokensJsonConverter))]
public sealed record SemanticTokensDeltaOrSemanticTokens
{
    public SemanticTokensDelta? TokensDelta { get; init; }
    public SemanticTokens? Tokens { get; init; }
}

public class SemanticTokensDeltaOrSemanticTokensJsonConverter : JsonConverter<SemanticTokensDeltaOrSemanticTokens>
{
    public override SemanticTokensDeltaOrSemanticTokens? Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        var jsonObj = JsonObject.Parse(ref reader);
        if (jsonObj?["edits"] is not null)
            return new() { TokensDelta = jsonObj?.Deserialize<SemanticTokensDelta>(options) };
        if (jsonObj?["data"] is not null)
            return new() { Tokens = jsonObj?.Deserialize<SemanticTokens>(options) };
        return new();
    }

    public override void Write(Utf8JsonWriter writer, SemanticTokensDeltaOrSemanticTokens value,
        JsonSerializerOptions options)
    {
        throw new NotSupportedException("It only use Deserialize.");
    }
}