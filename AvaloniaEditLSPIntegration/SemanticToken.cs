namespace AvaloniaEditLSPIntegration;

internal record SemanticToken
{
    private SemanticToken()
    {
    }

    public uint Length { get; private set; }
    public uint TokenType { get; private set; }
    public uint TokenModifiers { get; private set; }
    public uint Line { get; private set; }
    public uint StartCharacter { get; private set; }

    public static List<SemanticToken> Pares(IList<uint> tokens, uint startLineNumber = 1)
    {
        if (tokens.Count % 5 != 0)
            throw new AggregateException("Invalid tokens");
        var result = new List<SemanticToken>(tokens.Count / 5);
        for (var i = 0; i < tokens.Count; i += 5)
            result.Add(new SemanticToken
            {
                Line = tokens[i],
                StartCharacter = tokens[i + 1],
                Length = tokens[i + 2],
                TokenType = tokens[i + 3],
                TokenModifiers = tokens[i + 4]
            });

        if (result.Count != 0)
            result[0].Line += startLineNumber;
        for (var i = 1; i < result.Count; i++)
        {
            result[i].Line += result[i - 1].Line;
            if (result[i].Line == result[i - 1].Line)
                result[i].StartCharacter += result[i - 1].StartCharacter;
        }

        return result;
    }
}