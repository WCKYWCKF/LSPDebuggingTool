using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using EmmyLua.LanguageServer.Framework.Protocol.Message.Completion;

namespace WCKYWCKF.EmmyLua.LanguageServer.Framework.ClientEx.Union;

[JsonConverter(typeof(CompletionItemListOrCompletionListJsonConverter))]
public record CompletionItemListOrCompletionList()
{
    public List<CompletionItem>? CompletionItems { get; init; }
    public CompletionList? CompletionList { get; init; }
}

public class CompletionItemListOrCompletionListJsonConverter : JsonConverter<CompletionItemListOrCompletionList>
{
    public override CompletionItemListOrCompletionList? Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        var jsonNode = JsonNode.Parse(ref reader);
        if (jsonNode is JsonArray)
            return new() { CompletionItems = jsonNode.Deserialize<List<CompletionItem>>() };
        if (jsonNode?["isIncomplete"] != null)
            return new() { CompletionList = jsonNode.Deserialize<CompletionList>() };

        return new();
    }

    public override void Write(Utf8JsonWriter writer, CompletionItemListOrCompletionList value,
        JsonSerializerOptions options)
    {
        throw new NotSupportedException("It only use Deserialize.");
    }
}