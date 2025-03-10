﻿using System.Text.Json.Serialization;
using EmmyLua.LanguageServer.Framework.Protocol.Message.DocumentSymbol;
using WCKYWCKF.LanguageClient.Union;

namespace WCKYWCKF.LanguageClient;

[JsonSerializable(typeof(MarkedStringsOrMarkupContent))]
[JsonSerializable(typeof(SemanticTokensDeltaOrSemanticTokens))]
[JsonSerializable(typeof(RangeOrPrepareRenameResult))]
[JsonSerializable(typeof(SemanticTokensOrSemanticTokensDelta))]
[JsonSerializable(typeof(CompletionItemListOrCompletionList))]
[JsonSerializable(typeof(InlineCompletionItemListOrInlineCompletionList))]
[JsonSerializable(typeof(DocumentSymbolListOrSymbolInformationList))]
[JsonSerializable(typeof(LocationOrLocationListOrLocationLinkList))]
[JsonSerializable(typeof(SymbolInformationListOrWorkspaceSymbolList))]
public partial class ClientExJSC : JsonSerializerContext;