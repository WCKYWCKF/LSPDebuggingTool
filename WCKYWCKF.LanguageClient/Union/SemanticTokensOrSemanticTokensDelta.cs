using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using EmmyLua.LanguageServer.Framework.Protocol.Message.SemanticToken;

namespace WCKYWCKF.LanguageClient.Union;

[JsonConverter(typeof(SemanticTokensOrSemanticTokensDeltaJsonConverter))]
public record SemanticTokensOrSemanticTokensDelta()
{
    public SemanticTokensDelta? TokensDelta { get; init; }
    public SemanticTokens? Tokens { get; init; }
}

public class SemanticTokensOrSemanticTokensDeltaJsonConverter : JsonConverter<SemanticTokensOrSemanticTokensDelta>
{
    public override SemanticTokensOrSemanticTokensDelta? Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        var jsonObj = JsonObject.Parse(ref reader);
        if (jsonObj?["edits"] != null)
            return new() { TokensDelta = jsonObj.Deserialize<SemanticTokensDelta>(options) };
        if (jsonObj?["data"] != null)
            return new() { Tokens = jsonObj.Deserialize<SemanticTokens>(options) };
        return null;
    }

    public override void Write(Utf8JsonWriter writer, SemanticTokensOrSemanticTokensDelta value,
        JsonSerializerOptions options)
    {
        throw new NotSupportedException("It only use Deserialize.");
    }
}