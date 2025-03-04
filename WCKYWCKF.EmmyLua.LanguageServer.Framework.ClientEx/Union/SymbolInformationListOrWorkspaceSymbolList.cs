using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using EmmyLua.LanguageServer.Framework.Protocol.Message.DocumentSymbol;
using EmmyLua.LanguageServer.Framework.Protocol.Message.WorkspaceSymbol;
using WCKYWCKF.EmmyLua.LanguageServer.Framework.ClientEx.Protocol;

namespace WCKYWCKF.EmmyLua.LanguageServer.Framework.ClientEx.Union;

[JsonConverter(typeof(SymbolInformationListOrWorkspaceSymbolListJsonConverter))]
public record SymbolInformationListOrWorkspaceSymbolList()
{
    public List<SymbolInformation>? SymbolInformations { get; init; }
    public List<WorkspaceSymbol>? WorkspaceSymbols { get; init; }
}

public class
    SymbolInformationListOrWorkspaceSymbolListJsonConverter : JsonConverter<SymbolInformationListOrWorkspaceSymbolList>
{
    public override SymbolInformationListOrWorkspaceSymbolList? Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        var jsonArray = JsonNode.Parse(ref reader) as JsonArray;
        if (jsonArray?.Count is null or 0) return new();
        if (jsonArray[0]?["containerName"] != null)
            return new() { WorkspaceSymbols = jsonArray.Deserialize<List<WorkspaceSymbol>>() };
        if (jsonArray[0]?["location"] != null)
            return new() { SymbolInformations = jsonArray.Deserialize<List<SymbolInformation>>() };
        return new();
    }

    public override void Write(Utf8JsonWriter writer, SymbolInformationListOrWorkspaceSymbolList value,
        JsonSerializerOptions options)
    {
        throw new NotSupportedException("It only use Deserialize.");
    }
}