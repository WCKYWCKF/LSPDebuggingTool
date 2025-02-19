using EmmyLua.LanguageServer.Framework.Protocol.Message.SemanticToken;

namespace WCKYWCKF.EmmyLua.LanguageServer.Framework.ClientEx;

public sealed record SemanticTokensDeltaOrSemanticTokens(
    SemanticTokensDelta? TokensDelta,
    SemanticTokens? Tokens
);