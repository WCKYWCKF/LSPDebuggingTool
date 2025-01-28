using System.Collections.Generic;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace LSPDebuggingTool.ViewModels;

public class SemanticToken
{
    public static List<SemanticToken> Pares(SemanticTokens tokens)
    {
        var list = tokens.Data;
        var result = new List<SemanticToken>(list.Length / 5);
        for (int i = 0; i < list.Length; i += 5)
        {
            result.Add(new SemanticToken
            {
                Line = list[i],
                StartCharacter = list[i + 1],
                Length = list[i + 2],
                TokenType = list[i + 3],
                TokenModifiers = list[i + 4]
            });
        }

        if (result.Count != 0)
            result[0].Line += 1;
        for (int i = 1; i < result.Count; i++)
        {
            result[i].Line += result[i - 1].Line;
            if (result[i].Line == result[i - 1].Line)
                result[i].StartCharacter += result[i - 1].StartCharacter;
        }

        return result;
    }

    private SemanticToken()
    {
    }

    public int Line { get; private set; }
    public int StartCharacter { get; private set; }
    public int Length { get; private set; }
    public int TokenType { get; private set; }
    public int TokenModifiers { get; private set; }
}