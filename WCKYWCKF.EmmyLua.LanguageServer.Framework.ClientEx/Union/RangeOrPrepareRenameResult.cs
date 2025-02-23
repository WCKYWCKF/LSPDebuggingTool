using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using EmmyLua.LanguageServer.Framework.Protocol.Model;

namespace WCKYWCKF.EmmyLua.LanguageServer.Framework.ClientEx.Union;

[JsonConverter(typeof(RangeOrPrepareRenameResultJsonConverter))]
public record RangeOrPrepareRenameResult
{
    public DocumentRange? Range { get; init; }
    public PrepareRenameResult? PrepareRenameResult { get; init; }
}

public class RangeOrPrepareRenameResultJsonConverter : JsonConverter<RangeOrPrepareRenameResult>
{
    public override RangeOrPrepareRenameResult? Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        var jsonObj = JsonObject.Parse(ref reader);
        if (jsonObj?["range"] != null)
            return new() { Range = jsonObj.Deserialize<DocumentRange>(options) };
        if (jsonObj?["start"] != null)
            return new() { PrepareRenameResult = jsonObj.Deserialize<PrepareRenameResult>(options) };
        return null;
    }

    public override void Write(Utf8JsonWriter writer, RangeOrPrepareRenameResult value, JsonSerializerOptions options)
    {
        throw new NotSupportedException("It only use Deserialize.");
    }
}