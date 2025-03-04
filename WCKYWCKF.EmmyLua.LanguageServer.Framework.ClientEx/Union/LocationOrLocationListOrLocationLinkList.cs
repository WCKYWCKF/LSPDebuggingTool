using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using EmmyLua.LanguageServer.Framework.Protocol.Model;

namespace WCKYWCKF.EmmyLua.LanguageServer.Framework.ClientEx.Union;

[JsonConverter(typeof(LocationOrLocationListOrLocationLinkListJsonConverter))]
public record LocationOrLocationListOrLocationLinkList()
{
    public Location? Location { get; init; }
    public List<Location>? Locations { get; init; }
    public List<LocationLink>? LocationLinks { get; init; }
}

public class
    LocationOrLocationListOrLocationLinkListJsonConverter : JsonConverter<LocationOrLocationListOrLocationLinkList>
{
    public override LocationOrLocationListOrLocationLinkList? Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        var jsonNode = JsonNode.Parse(ref reader);
        if (jsonNode is JsonObject)
            return new() { Location = jsonNode.Deserialize<Location>() };
        if (jsonNode is JsonArray jsonArray && jsonArray.Count != 0)
        {
            if (jsonArray[0]?["uri"] != null)
                return new() { Locations = jsonArray.Deserialize<List<Location>>() };
            if (jsonArray[0]?["originSelectionRange"] != null)
                return new() { LocationLinks = jsonArray.Deserialize<List<LocationLink>>() };
        }

        return new();
    }

    public override void Write(Utf8JsonWriter writer, LocationOrLocationListOrLocationLinkList value,
        JsonSerializerOptions options)
    {
        throw new NotSupportedException("It only use Deserialize.");
    }
}