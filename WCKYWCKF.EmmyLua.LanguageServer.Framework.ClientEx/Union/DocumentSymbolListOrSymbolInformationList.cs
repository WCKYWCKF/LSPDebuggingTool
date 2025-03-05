using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using EmmyLua.LanguageServer.Framework.Protocol.Message.DocumentSymbol;
using EmmyLua.LanguageServer.Framework.Protocol.Supplement;

namespace WCKYWCKF.EmmyLua.LanguageServer.Framework.ClientEx.Union;

[JsonConverter(typeof(DocumentSymbolListOrSymbolInformationListJsonConverter))]
public record DocumentSymbolListOrSymbolInformationList()
{
    public List<DocumentSymbol>? DocumentSymbols { get; init; }
    public List<SymbolInformation>? SymbolInformations { get; init; }
}

public class
    DocumentSymbolListOrSymbolInformationListJsonConverter : JsonConverter<DocumentSymbolListOrSymbolInformationList>
{
    public override DocumentSymbolListOrSymbolInformationList? Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        var jsonArray = JsonNode.Parse(ref reader) as JsonArray;
        if (jsonArray?.Count is null or 0) return new();
        if (jsonArray[0]?["range"] != null)
            return new() { DocumentSymbols = jsonArray.Deserialize<List<DocumentSymbol>>(options) };
        if (jsonArray[0]?["location"] != null)
            return new() { SymbolInformations = jsonArray.Deserialize<List<SymbolInformation>>(options) };
        return new();
    }

    public override void Write(Utf8JsonWriter writer, DocumentSymbolListOrSymbolInformationList value,
        JsonSerializerOptions options)
    {
        throw new NotSupportedException("It only use Deserialize.");
    }
}