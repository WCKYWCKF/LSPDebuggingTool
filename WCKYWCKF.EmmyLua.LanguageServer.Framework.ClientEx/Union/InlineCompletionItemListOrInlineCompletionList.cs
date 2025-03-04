using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using EmmyLua.LanguageServer.Framework.Protocol.Message.InlineCompletion;

namespace WCKYWCKF.EmmyLua.LanguageServer.Framework.ClientEx.Union;

[JsonConverter(typeof(InlineCompletionItemListOrInlineCompletionListJsonConverter))]
public record InlineCompletionItemListOrInlineCompletionList()
{
    public List<InlineCompletionItem>? InlineCompletionItems { get; init; }
    public InlineCompletionList? InlineCompletionList { get; init; }
}

public class
    InlineCompletionItemListOrInlineCompletionListJsonConverter : JsonConverter<
    InlineCompletionItemListOrInlineCompletionList>
{
    public override InlineCompletionItemListOrInlineCompletionList? Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        var jsonNode = JsonNode.Parse(ref reader);
        if (jsonNode is JsonArray)
            return new() { InlineCompletionItems = jsonNode.Deserialize<List<InlineCompletionItem>>() };
        if (jsonNode?["items"] != null)
            return new() { InlineCompletionList = jsonNode.Deserialize<InlineCompletionList>(options) };
        return new();
    }

    public override void Write(Utf8JsonWriter writer, InlineCompletionItemListOrInlineCompletionList value,
        JsonSerializerOptions options)
    {
        throw new NotSupportedException("It only use Deserialize.");
    }
}