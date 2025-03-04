using System.Text.Json.Serialization;
using EmmyLua.LanguageServer.Framework.Protocol.Message.DocumentSymbol;
using WCKYWCKF.EmmyLua.LanguageServer.Framework.ClientEx.Protocol;
using WCKYWCKF.EmmyLua.LanguageServer.Framework.ClientEx.Union;

namespace WCKYWCKF.EmmyLua.LanguageServer.Framework.ClientEx;

[JsonSerializable(typeof(ShowDocumentResult))]
[JsonSerializable(typeof(MessageActionItem))]
[JsonSerializable(typeof(Moniker))]
[JsonSerializable(typeof(ShowDocumentParams))]
[JsonSerializable(typeof(ShowMessageRequestParams))]
[JsonSerializable(typeof(MonikerParams))]
[JsonSerializable(typeof(MonikerKind))]
[JsonSerializable(typeof(UniquenessLevel))]
[JsonSerializable(typeof(ProgressToken))]
[JsonSerializable(typeof(WorkDoneProgressCancelParams))]
[JsonSerializable(typeof(WorkDoneProgressCreateParams))]
[JsonSerializable(typeof(TextDocumentContentParams))]
[JsonSerializable(typeof(TextDocumentContentResult))]
[JsonSerializable(typeof(LogTraceParams))]
[JsonSerializable(typeof(TextDocumentContentRefreshParams))]
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
[JsonSerializable(typeof(CompletionItemListOrCompletionList))]
[JsonSerializable(typeof(InlineCompletionItemListOrInlineCompletionList))]
[JsonSerializable(typeof(DocumentSymbolListOrSymbolInformationList))]
[JsonSerializable(typeof(LocationOrLocationListOrLocationLinkList))]
[JsonSerializable(typeof(SymbolInformationListOrWorkspaceSymbolList))]
public partial class ClientExJSC : JsonSerializerContext;