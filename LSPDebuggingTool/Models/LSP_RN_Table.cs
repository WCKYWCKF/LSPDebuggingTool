using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LSPDebuggingTool.Models;

public static class LSP_RN_Table
{
    public const string SourceJsonStr =
        """
        [
            {
                "Function": "Server lifecycle",
                "Name": "Initialize Request",
                "Method": "initialize",
                "ParameterName": "InitializeParams",
                "ReturnValue": "InitializeResult",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "The initialize request is sent as the first request from the client to the server. If the server receives a request or notification before the `initialize` request, it should act as follows:\n* For a request, the response should be an error with `code: -32002`. The message can be picked by the server.\n* Notifications should be dropped, except for the exit notification. This will allow the exit of a server without an initialize request.\nUntil the server has responded to the `initialize` request with an `InitializeResult`, the client must not send any additional requests or notifications to the server. In addition the server is not allowed to send any requests or notifications to the client until it has responded with an `InitializeResult`, with the exception that during the `initialize` request the server is allowed to send the notifications `window/showMessage`, `window/logMessage` and `telemetry/event` as well as the `window/showMessageRequest` request to the client. In case the client sets up a progress token in the initialize params (e.g. property `workDoneToken`) the server is also allowed to use that token (and only that token) using the `$/progress` notification sent from the server to the client.\nThe `initialize` request may only be sent once."
            },
            {
                "Function": "Server lifecycle",
                "Name": "Initialized Notification",
                "Method": "initialized",
                "ParameterName": "InitializedParams",
                "ReturnValue": "这是一个通知",
                "R_Or_N": "Notification（通知）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "The initialized notification is sent from the client to the server after the client received the result of the `initialize` request, but before the client is sending any other request or notification to the server. The server can use the `initialized` notification, for example, to dynamically register capabilities. The `initialized` notification may only be sent once."
            },
            {
                "Function": "Server lifecycle",
                "Name": "Register Capability",
                "Method": "client/registerCapability",
                "ParameterName": "RegistrationParams",
                "ReturnValue": "null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "The `client/registerCapability` request is sent from the server to the client to register for a new capability on the client side. Not all clients need to support dynamic capability registration. A client opts in via the `dynamicRegistration` property on the specific client capabilities. A client can even provide dynamic registration for capability A but not for capability B (see `TextDocumentClientCapabilities` as an example).\nServer must not register the same capability both statically through the initialize result and dynamically for the same document selector. If a server wants to support both static and dynamic registration it needs to check the client capability in the initialize request and only register the capability statically if the client doesn't support dynamic registration for that capability."
            },
            {
                "Function": "Server lifecycle",
                "Name": "Unregister Capability",
                "Method": "client/unregisterCapability",
                "ParameterName": "UnregistrationParams",
                "ReturnValue": "null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "The `client/unregisterCapability` request is sent from the server to the client to unregister a previously registered capability."
            },
            {
                "Function": "Server lifecycle",
                "Name": "SetTrace Notification",
                "Method": "$/setTrace",
                "ParameterName": "SetTraceParams",
                "ReturnValue": "这是一个通知",
                "R_Or_N": "Notification（通知）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "A notification that should be used by the client to modify the trace setting of the server."
            },
            {
                "Function": "Server lifecycle",
                "Name": "LogTrace Notification",
                "Method": "$/logTrace",
                "ParameterName": "LogTraceParams",
                "ReturnValue": "这是一个通知",
                "R_Or_N": "Notification（通知）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "A notification to log the trace of the server's execution.\nThe amount and content of these notifications depends on the current `trace` configuration.\nIf `trace` is `'off'`, the server should not send any `logTrace` notification.\nIf `trace` is `'messages'`, the server should not add the `'verbose'` field in the `LogTraceParams`.\n`$/logTrace` should be used for systematic trace reporting. For single debugging messages, the server should send [`window/logMessage`](#window_logMessage) notifications."
            },
            {
                "Function": "Server lifecycle",
                "Name": "Shutdown Request",
                "Method": "shutdown",
                "ParameterName": "none",
                "ReturnValue": "null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "The shutdown request is sent from the client to the server. It asks the server to shut down, but to not exit (otherwise the response might not be delivered correctly to the client). There is a separate exit notification that asks the server to exit. Clients must not send any notifications other than `exit` or requests to a server to which they have sent a shutdown request. Clients should also wait with sending the `exit` notification until they have received a response from the `shutdown` request."
            },
            {
                "Function": "Server lifecycle",
                "Name": "Exit Notification",
                "Method": "exit",
                "ParameterName": "none",
                "ReturnValue": "这是一个通知",
                "R_Or_N": "Notification（通知）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "A notification to ask the server to exit its process.\nThe server should exit with `success` code 0 if the shutdown request has been received before; otherwise with `error` code 1."
            },
            {
                "Function": "Text Document Synchronization",
                "Name": "DidOpenTextDocument Notification",
                "Method": "textDocument/didOpen",
                "ParameterName": "DidOpenTextDocumentParams",
                "ReturnValue": "这是一个通知",
                "R_Or_N": "Notification（通知）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "The document open notification is sent from the client to the server to signal newly opened text documents. The document's content is now managed by the client and the server must not try to read the document's content using the document's Uri. Open in this sense means it is managed by the client. It doesn't necessarily mean that its content is presented in an editor. An open notification must not be sent more than once without a corresponding close notification send before. This means open and close notification must be balanced and the max open count for a particular textDocument is one. Note that a server's ability to fulfill requests is independent of whether a text document is open or closed.\nThe `DidOpenTextDocumentParams` contain the language id the document is associated with. If the language id of a document changes, the client needs to send a `textDocument/didClose` to the server followed by a `textDocument/didOpen` with the new language id if the server handles the new language id as well."
            },
            {
                "Function": "Text Document Synchronization",
                "Name": "DidChangeTextDocument Notification",
                "Method": "textDocument/didChange",
                "ParameterName": "DidChangeTextDocumentParams",
                "ReturnValue": "这是一个通知",
                "R_Or_N": "Notification（通知）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "The document change notification is sent from the client to the server to signal changes to a text document. Before a client can change a text document it must claim ownership of its content using the `textDocument/didOpen` notification. In 2.0 the shape of the params has changed to include proper version numbers."
            },
            {
                "Function": "Text Document Synchronization",
                "Name": "WillSaveTextDocument Notification",
                "Method": "textDocument/willSave",
                "ParameterName": "WillSaveTextDocumentParams",
                "ReturnValue": "这是一个通知",
                "R_Or_N": "Notification（通知）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "The document will save notification is sent from the client to the server before the document is actually saved. If a server has registered for open / close events clients should ensure that the document is open before a `willSave` notification is sent since clients can't change the content of a file without ownership transferal."
            },
            {
                "Function": "Text Document Synchronization",
                "Name": "WillSaveWaitUntilTextDocument Request",
                "Method": "textDocument/willSaveWaitUntil",
                "ParameterName": "WillSaveTextDocumentParams",
                "ReturnValue": "TextEdit[] | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "The document will save request is sent from the client to the server before the document is actually saved. The request can return an array of TextEdits which will be applied to the text document before it is saved. Please note that clients might drop results if computing the text edits took too long or if a server constantly fails on this request. This is done to keep the save fast and reliable.  If a server has registered for open / close events clients should ensure that the document is open before a `willSaveWaitUntil` notification is sent since clients can't change the content of a file without ownership transferal."
            },
            {
                "Function": "Text Document Synchronization",
                "Name": "DidSaveTextDocument Notification",
                "Method": "textDocument/didSave",
                "ParameterName": "DidSaveTextDocumentParams",
                "ReturnValue": "这是一个通知",
                "R_Or_N": "Notification（通知）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "The document save notification is sent from the client to the server when the document was saved in the client."
            },
            {
                "Function": "Text Document Synchronization",
                "Name": "DidCloseTextDocument Notification",
                "Method": "textDocument/didClose",
                "ParameterName": "DidCloseTextDocumentParams",
                "ReturnValue": "这是一个通知",
                "R_Or_N": "Notification（通知）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "The document close notification is sent from the client to the server when the document got closed in the client. The document's master now exists where the document's Uri points to (e.g. if the document's Uri is a file Uri the master now exists on disk). As with the open notification the close notification is about managing the document's content. Receiving a close notification doesn't mean that the document was open in an editor before. A close notification requires a previous open notification to be sent. Note that a server's ability to fulfill requests is independent of whether a text document is open or closed."
            },
            {
                "Function": "Notebook Document Synchronization",
                "Name": "DidOpenNotebookDocument Notification",
                "Method": "notebookDocument/didOpen",
                "ParameterName": "DidOpenNotebookDocumentParams",
                "ReturnValue": "这是一个通知",
                "R_Or_N": "Notification（通知）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "The open notification is sent from the client to the server when a notebook document is opened. It is only sent by a client if the server requested the synchronization mode `notebook` in its `notebookDocumentSync` capability."
            },
            {
                "Function": "Notebook Document Synchronization",
                "Name": "DidChangeNotebookDocument Notification",
                "Method": "notebookDocument/didChange",
                "ParameterName": "DidChangeNotebookDocumentParams",
                "ReturnValue": "这是一个通知",
                "R_Or_N": "Notification（通知）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "The change notification is sent from the client to the server when a notebook document changes. It is only sent by a client if the server requested the synchronization mode `notebook` in its `notebookDocumentSync` capability."
            },
            {
                "Function": "Notebook Document Synchronization",
                "Name": "DidSaveNotebookDocument Notification",
                "Method": "notebookDocument/didSave",
                "ParameterName": "DidSaveNotebookDocumentParams",
                "ReturnValue": "这是一个通知",
                "R_Or_N": "Notification（通知）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "The save notification is sent from the client to the server when a notebook document is saved. It is only sent by a client if the server requested the synchronization mode `notebook` in its `notebookDocumentSync` capability."
            },
            {
                "Function": "Notebook Document Synchronization",
                "Name": "DidCloseNotebookDocument Notification",
                "Method": "notebookDocument/didClose",
                "ParameterName": "DidCloseNotebookDocumentParams",
                "ReturnValue": "这是一个通知",
                "R_Or_N": "Notification（通知）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "The close notification is sent from the client to the server when a notebook document is closed. It is only sent by a client if the server requested the synchronization mode `notebook` in its `notebookDocumentSync` capability."
            },
            {
                "Function": "Language Features",
                "Name": "Go to Declaration Request",
                "Method": "textDocument/declaration",
                "ParameterName": "DeclarationParams",
                "ReturnValue": "Location | Location[] | LocationLink[] | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "The go to declaration request is sent from the client to the server to resolve the declaration location of a symbol at a given text document position.\nThe result type [`LocationLink`](#locationLink)[] got introduced with version 3.14.0 and depends on the corresponding client capability `textDocument.declaration.linkSupport`."
            },
            {
                "Function": "Language Features",
                "Name": "Go to Definition Request",
                "Method": "textDocument/definition",
                "ParameterName": "DefinitionParams",
                "ReturnValue": "Location | Location[] | LocationLink[] | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "The go to definition request is sent from the client to the server to resolve the definition location of a symbol at a given text document position.\nThe result type [`LocationLink`](#locationLink)[] got introduced with version 3.14.0 and depends on the corresponding client capability `textDocument.definition.linkSupport`."
            },
            {
                "Function": "Language Features",
                "Name": "Go to Type Definition Request",
                "Method": "textDocument/typeDefinition",
                "ParameterName": "TypeDefinitionParams",
                "ReturnValue": "Location | Location[] | LocationLink[] | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Go to Implementation Request",
                "Method": "textDocument/implementation",
                "ParameterName": "ImplementationParams",
                "ReturnValue": "Location | Location[] | LocationLink[] | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Find References Request",
                "Method": "textDocument/references",
                "ParameterName": "ReferenceParams",
                "ReturnValue": "Location[] | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Prepare Call Hierarchy Request",
                "Method": "textDocument/prepareCallHierarchy",
                "ParameterName": "CallHierarchyPrepareParams",
                "ReturnValue": "CallHierarchyItem[] | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Call Hierarchy Incoming Calls",
                "Method": "callHierarchy/incomingCalls",
                "ParameterName": "CallHierarchyIncomingCallsParams",
                "ReturnValue": "CallHierarchyIncomingCall[] | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Call Hierarchy Outgoing Calls",
                "Method": "callHierarchy/outgoingCalls",
                "ParameterName": "CallHierarchyOutgoingCallsParams",
                "ReturnValue": "CallHierarchyOutgoingCall[] | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Prepare Type Hierarchy Request",
                "Method": "textDocument/prepareTypeHierarchy",
                "ParameterName": "TypeHierarchyPrepareParams",
                "ReturnValue": "TypeHierarchyItem[] | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Type Hierarchy Supertypes",
                "Method": "typeHierarchy/supertypes",
                "ParameterName": "TypeHierarchySupertypesParams",
                "ReturnValue": "TypeHierarchyItem[] | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Type Hierarchy Subtypes",
                "Method": "typeHierarchy/subtypes",
                "ParameterName": "TypeHierarchySubtypesParams",
                "ReturnValue": "TypeHierarchyItem[] | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Document Highlights Request",
                "Method": "textDocument/documentHighlight",
                "ParameterName": "DocumentHighlightParams",
                "ReturnValue": "DocumentHighlight[] | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Document Link Request",
                "Method": "textDocument/documentLink",
                "ParameterName": "DocumentLinkParams",
                "ReturnValue": "DocumentLink[] | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Document Link Resolve Request",
                "Method": "documentLink/resolve",
                "ParameterName": "DocumentLink",
                "ReturnValue": "DocumentLink",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Hover Request",
                "Method": "textDocument/hover",
                "ParameterName": "HoverParams",
                "ReturnValue": "Hover | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Code Lens Request",
                "Method": "textDocument/codeLens",
                "ParameterName": "CodeLensParams",
                "ReturnValue": "CodeLens[] | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Code Lens Resolve Request",
                "Method": "codeLens/resolve",
                "ParameterName": "CodeLens",
                "ReturnValue": "CodeLens",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Code Lens Refresh Request",
                "Method": "workspace/codeLens/refresh",
                "ParameterName": "none",
                "ReturnValue": "这是一个通知",
                "R_Or_N": "Notification（通知）",
                "DirectionTransmission": "Server To Client",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Folding Range Request",
                "Method": "textDocument/foldingRange",
                "ParameterName": "FoldingRangeParams",
                "ReturnValue": "FoldingRange[] | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Folding Range Refresh Request",
                "Method": "workspace/foldingRange/refresh",
                "ParameterName": "none",
                "ReturnValue": "这是一个通知",
                "R_Or_N": "Notification（通知）",
                "DirectionTransmission": "Server To Client",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Selection Range Request",
                "Method": "textDocument/selectionRange",
                "ParameterName": "SelectionRangeParams",
                "ReturnValue": "SelectionRange[] | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Document Symbols Request",
                "Method": "textDocument/documentSymbol",
                "ParameterName": "DocumentSymbolParams",
                "ReturnValue": "DocumentSymbol[] | SymbolInformation[] | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Semantic Tokens For Full File",
                "Method": "textDocument/semanticTokens/full",
                "ParameterName": "SemanticTokensParams",
                "ReturnValue": "SemanticTokens | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Semantic Tokens For Delta File",
                "Method": "textDocument/semanticTokens/full/delta",
                "ParameterName": "SemanticTokensDeltaParams",
                "ReturnValue": "SemanticTokens | SemanticTokensDelta | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Semantic Tokens For Range File",
                "Method": "textDocument/semanticTokens/range",
                "ParameterName": "SemanticTokensRangeParams",
                "ReturnValue": "SemanticTokens | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Semantic Tokens For Refresh File",
                "Method": "workspace/semanticTokens/refresh",
                "ParameterName": "none",
                "ReturnValue": "这是一个通知",
                "R_Or_N": "Notification（通知）",
                "DirectionTransmission": "Server To Client",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Inlay Hint Request",
                "Method": "textDocument/inlayHint",
                "ParameterName": "InlayHintParams",
                "ReturnValue": "InlayHint[] | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Inlay Hint Resolve Request",
                "Method": "inlayHint/resolve",
                "ParameterName": "InlayHint",
                "ReturnValue": "InlayHint",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Inlay Hint Refresh Request",
                "Method": "workspace/inlayHint/refresh",
                "ParameterName": "none",
                "ReturnValue": "这是一个通知",
                "R_Or_N": "Notification（通知）",
                "DirectionTransmission": "Server To Client",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Inline Value Request",
                "Method": "textDocument/inlineValue",
                "ParameterName": "InlineValueParams",
                "ReturnValue": "InlineValue[] | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Inline Value Refresh Request",
                "Method": "workspace/inlineValue/refresh",
                "ParameterName": "none",
                "ReturnValue": "这是一个通知",
                "R_Or_N": "Notification（通知）",
                "DirectionTransmission": "Server To Client",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Monikers",
                "Method": "textDocument/moniker",
                "ParameterName": "MonikerParams",
                "ReturnValue": "Moniker[] | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Completion Request",
                "Method": "textDocument/completion",
                "ParameterName": "CompletionParams",
                "ReturnValue": "CompletionItem[] | CompletionList | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Completion Item Resolve Request",
                "Method": "completionItem/resolve",
                "ParameterName": "CompletionItem",
                "ReturnValue": "CompletionItem",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "PublishDiagnostics Notification",
                "Method": "textDocument/publishDiagnostics",
                "ParameterName": "PublishDiagnosticsParams",
                "ReturnValue": "这是一个通知",
                "R_Or_N": "Notification（通知）",
                "DirectionTransmission": "Server To Client",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Document Diagnostics",
                "Method": "textDocument/diagnostic",
                "ParameterName": "DocumentDiagnosticParams",
                "ReturnValue": "DocumentDiagnosticReport",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Workspace Diagnostics",
                "Method": "workspace/diagnostic",
                "ParameterName": "WorkspaceDiagnosticParams",
                "ReturnValue": "WorkspaceDiagnosticReport",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Diagnostics Refresh",
                "Method": "workspace/diagnostic/refresh",
                "ParameterName": "none",
                "ReturnValue": "这是一个通知",
                "R_Or_N": "Notification（通知）",
                "DirectionTransmission": "Server To Client",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Signature Help Request",
                "Method": "textDocument/signatureHelp",
                "ParameterName": "SignatureHelpParams",
                "ReturnValue": "SignatureHelp | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Code Action Request",
                "Method": "textDocument/codeAction",
                "ParameterName": "CodeActionParams",
                "ReturnValue": "(Command | CodeAction)[] | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Code Action Resolve Request",
                "Method": "codeAction/resolve",
                "ParameterName": "CodeAction",
                "ReturnValue": "CodeAction",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Document Color Request",
                "Method": "textDocument/documentColor",
                "ParameterName": "DocumentColorParams",
                "ReturnValue": "ColorInformation[]",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Color Presentation Request",
                "Method": "textDocument/colorPresentation",
                "ParameterName": "ColorPresentationParams",
                "ReturnValue": "ColorPresentation[]",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Document Formatting Request",
                "Method": "textDocument/formatting",
                "ParameterName": "DocumentFormattingParams",
                "ReturnValue": "TextEdit[] | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Document Range Formatting Request",
                "Method": "textDocument/rangeFormatting",
                "ParameterName": "DocumentRangeFormattingParams",
                "ReturnValue": "TextEdit[] | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Document on Type Formatting Request",
                "Method": "textDocument/onTypeFormatting",
                "ParameterName": "DocumentOnTypeFormattingParams",
                "ReturnValue": "TextEdit[] | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Rename Request",
                "Method": "textDocument/rename",
                "ParameterName": "RenameParams",
                "ReturnValue": "WorkspaceEdit | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Prepare Rename Request",
                "Method": "textDocument/prepareRename",
                "ParameterName": "PrepareRenameParams",
                "ReturnValue": "Range | { range: Range, placeholder: string } | { defaultBehavior: boolean } | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Linked Editing Range",
                "Method": "textDocument/linkedEditingRange",
                "ParameterName": "LinkedEditingRangeParams",
                "ReturnValue": "LinkedEditingRanges | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Language Features",
                "Name": "Inline Completion Request",
                "Method": "textDocument/inlineCompletion",
                "ParameterName": "InlineCompletionParams",
                "ReturnValue": "InlineCompletionItem[] | InlineCompletionList | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Workspace Features",
                "Name": "Workspace Symbols Request",
                "Method": "workspace/symbol",
                "ParameterName": "WorkspaceSymbolParams",
                "ReturnValue": "SymbolInformation[] | WorkspaceSymbol[] | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Workspace Features",
                "Name": "Workspace Symbol Resolve Request",
                "Method": "workspaceSymbol/resolve",
                "ParameterName": "WorkspaceSymbol",
                "ReturnValue": "WorkspaceSymbol",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Workspace Features",
                "Name": "Configuration Request",
                "Method": "workspace/configuration",
                "ParameterName": "ConfigurationParams",
                "ReturnValue": "LSPAny[]",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Server To Client",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Workspace Features",
                "Name": "DidChangeConfiguration Notification",
                "Method": "workspace/didChangeConfiguration",
                "ParameterName": "DidChangeConfigurationParams",
                "ReturnValue": "这是一个通知",
                "R_Or_N": "Notification（通知）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Workspace Features",
                "Name": "Workspace Folders Request",
                "Method": "workspace/workspaceFolders",
                "ParameterName": "none",
                "ReturnValue": "WorkspaceFolder[] | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Server To Client",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Workspace Features",
                "Name": "DidChangeWorkspaceFolders Notification",
                "Method": "workspace/didChangeWorkspaceFolders",
                "ParameterName": "DidChangeWorkspaceFoldersParams",
                "ReturnValue": "这是一个通知",
                "R_Or_N": "Notification（通知）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Workspace Features",
                "Name": "WillCreateFiles Request",
                "Method": "workspace/willCreateFiles",
                "ParameterName": "CreateFilesParams",
                "ReturnValue": "WorkspaceEdit | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Workspace Features",
                "Name": "DidCreateFiles Notification",
                "Method": "workspace/didCreateFiles",
                "ParameterName": "CreateFilesParams",
                "ReturnValue": "这是一个通知",
                "R_Or_N": "Notification（通知）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Workspace Features",
                "Name": "WillRenameFiles Request",
                "Method": "workspace/willRenameFiles",
                "ParameterName": "RenameFilesParams",
                "ReturnValue": "WorkspaceEdit | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Workspace Features",
                "Name": "DidRenameFiles Notification",
                "Method": "workspace/didRenameFiles",
                "ParameterName": "RenameFilesParams",
                "ReturnValue": "这是一个通知",
                "R_Or_N": "Notification（通知）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Workspace Features",
                "Name": "WillDeleteFiles Request",
                "Method": "workspace/willDeleteFiles",
                "ParameterName": "DeleteFilesParams",
                "ReturnValue": "WorkspaceEdit | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Workspace Features",
                "Name": "DidDeleteFiles Notification",
                "Method": "workspace/didDeleteFiles",
                "ParameterName": "DeleteFilesParams",
                "ReturnValue": "这是一个通知",
                "R_Or_N": "Notification（通知）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Workspace Features",
                "Name": "DidChangeWatchedFiles Notification",
                "Method": "workspace/didChangeWatchedFiles",
                "ParameterName": "DidChangeWatchedFilesParams",
                "ReturnValue": "这是一个通知",
                "R_Or_N": "Notification（通知）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Workspace Features",
                "Name": "Execute a command",
                "Method": "workspace/executeCommand",
                "ParameterName": "ExecuteCommandParams",
                "ReturnValue": "LSPAny",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Workspace Features",
                "Name": "Applies a WorkspaceEdit",
                "Method": "workspace/applyEdit",
                "ParameterName": "ApplyWorkspaceEditParams",
                "ReturnValue": "ApplyWorkspaceEditResult",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Server To Client",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Workspace Features",
                "Name": "Text Document Content Request",
                "Method": "workspace/textDocumentContent",
                "ParameterName": "TextDocumentContentParams",
                "ReturnValue": "TextDocumentContentResult",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Client To Server",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Workspace Features",
                "Name": "Text Document Content Refresh Request",
                "Method": "workspace/textDocumentContent/refresh",
                "ParameterName": "TextDocumentContentRefreshParams",
                "ReturnValue": "null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Server To Client",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Window Features",
                "Name": "ShowMessage Notification",
                "Method": "window/showMessage",
                "ParameterName": "ShowMessageParams",
                "ReturnValue": "这是一个通知",
                "R_Or_N": "Notification（通知）",
                "DirectionTransmission": "Server To Client",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Window Features",
                "Name": "ShowMessage Request",
                "Method": "window/showMessageRequest",
                "ParameterName": "ShowMessageRequestParams",
                "ReturnValue": "MessageActionItem | null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Server To Client",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Window Features",
                "Name": "Show Document Request",
                "Method": "window/showDocument",
                "ParameterName": "ShowDocumentParams",
                "ReturnValue": "ShowDocumentResult",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Server To Client",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Window Features",
                "Name": "LogMessage Notification",
                "Method": "window/logMessage",
                "ParameterName": "LogMessageParams",
                "ReturnValue": "这是一个通知",
                "R_Or_N": "Notification（通知）",
                "DirectionTransmission": "Server To Client",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Window Features",
                "Name": "Create Work Done Progress",
                "Method": "window/workDoneProgress/create",
                "ParameterName": "WorkDoneProgressCreateParams",
                "ReturnValue": "null",
                "R_Or_N": "Request（请求）",
                "DirectionTransmission": "Server To Client",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Window Features",
                "Name": "Cancel a Work Done Progress",
                "Method": "window/workDoneProgress/cancel",
                "ParameterName": "WorkDoneProgressCancelParams",
                "ReturnValue": "这是一个通知",
                "R_Or_N": "Notification（通知）",
                "DirectionTransmission": "Server To Client",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Window Features",
                "Name": "Telemetry Notification",
                "Method": "telemetry/event",
                "ParameterName": "object | array",
                "ReturnValue": "这是一个通知",
                "R_Or_N": "Notification（通知）",
                "DirectionTransmission": "Server To Client",
                "DescriptionByMd": "未填充"
            },
            {
                "Function": "Miscellaneous",
                "Name": "Progress Support",
                "Method": "$/progress",
                "ParameterName": "ProgressParams",
                "ReturnValue": "这是一个通知",
                "R_Or_N": "Notification（通知）",
                "DirectionTransmission": "Two",
                "DescriptionByMd": "未填充"
            }
        ]
        """;

    public record LSP_RN_TableItem(
        string Function,
        string Name,
        string Method,
        string ParameterName,
        string ReturnValue,
        string R_Or_N,
        string DirectionTransmission,
        string DescriptionByMd);

    public static IReadOnlyList<LSP_RN_TableItem> TableItems { get; }

    static LSP_RN_Table()
    {
        var tableItems =
            JsonSerializer.Deserialize<List<LSP_RN_TableItem>>(SourceJsonStr, LSP_RN_TableJSC.Default.Options);
        TableItems = tableItems?.AsReadOnly() ?? new List<LSP_RN_TableItem>().AsReadOnly();
    }
}

[JsonSerializable(typeof(List<LSP_RN_Table.LSP_RN_TableItem>))]
internal partial class LSP_RN_TableJSC : JsonSerializerContext;