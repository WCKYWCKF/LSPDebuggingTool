using System.Text.Json.Serialization;
using EmmyLua.LanguageServer.Framework.Protocol.Message.DocumentSymbol;
using WCKYWCKF.EmmyLua.LanguageServer.Framework.ClientEx.Protocol;
using WCKYWCKF.EmmyLua.LanguageServer.Framework.ClientEx.Union;

namespace WCKYWCKF.EmmyLua.LanguageServer.Framework.ClientEx;

[JsonSerializable(typeof(Hover))]
[JsonSerializable(typeof(MarkedString))]
[JsonSerializable(typeof(List<MarkedString>))]
[JsonSerializable(typeof(MarkedStringsOrMarkupContent))]
[JsonSerializable(typeof(LogMessageParams))]
[JsonSerializable(typeof(MessageType))]
[JsonSerializable(typeof(DidChangeConfigurationParams))]
[JsonSerializable(typeof(SemanticTokensDeltaOrSemanticTokens))]
[JsonSerializable(typeof(SymbolInformation))]
[JsonSerializable(typeof(List<SymbolTag>))]
[JsonSerializable(typeof(PrepareRenameResult))]
[JsonSerializable(typeof(RangeOrPrepareRenameResult))]
[JsonSerializable(typeof(SemanticTokensOrSemanticTokensDelta))]
public partial class ClientExJSC : JsonSerializerContext;