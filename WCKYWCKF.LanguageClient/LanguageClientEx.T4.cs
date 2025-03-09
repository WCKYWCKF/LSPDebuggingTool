using EmmyLua.LanguageServer.Framework;
using EmmyLua.LanguageServer.Framework.Protocol.JsonRpc;
using EmmyLua.LanguageServer.Framework.Protocol.Message;
using EmmyLua.LanguageServer.Framework.Protocol.Message.CallHierarchy;
using EmmyLua.LanguageServer.Framework.Protocol.Message.Client.ApplyWorkspaceEdit;
using EmmyLua.LanguageServer.Framework.Protocol.Message.Client.PublishDiagnostics;
using EmmyLua.LanguageServer.Framework.Protocol.Message.Client.Registration;
using EmmyLua.LanguageServer.Framework.Protocol.Message.Client.ShowMessage;
using EmmyLua.LanguageServer.Framework.Protocol.Message.CodeAction;
using EmmyLua.LanguageServer.Framework.Protocol.Message.CodeLens;
using EmmyLua.LanguageServer.Framework.Protocol.Message.Completion;
using EmmyLua.LanguageServer.Framework.Protocol.Message.Configuration;
using EmmyLua.LanguageServer.Framework.Protocol.Message.Declaration;
using EmmyLua.LanguageServer.Framework.Protocol.Message.Definition;
using EmmyLua.LanguageServer.Framework.Protocol.Message.DocumentColor;
using EmmyLua.LanguageServer.Framework.Protocol.Message.DocumentDiagnostic;
using EmmyLua.LanguageServer.Framework.Protocol.Message.DocumentFormatting;
using EmmyLua.LanguageServer.Framework.Protocol.Message.DocumentHighlight;
using EmmyLua.LanguageServer.Framework.Protocol.Message.DocumentLink;
using EmmyLua.LanguageServer.Framework.Protocol.Message.DocumentSymbol;
using EmmyLua.LanguageServer.Framework.Protocol.Message.ExecuteCommand;
using EmmyLua.LanguageServer.Framework.Protocol.Message.FoldingRange;
using EmmyLua.LanguageServer.Framework.Protocol.Message.Hover;
using EmmyLua.LanguageServer.Framework.Protocol.Message.Implementation;
using EmmyLua.LanguageServer.Framework.Protocol.Message.Initialize;
using EmmyLua.LanguageServer.Framework.Protocol.Message.InlayHint;
using EmmyLua.LanguageServer.Framework.Protocol.Message.InlineCompletion;
using EmmyLua.LanguageServer.Framework.Protocol.Message.InlineValue;
using EmmyLua.LanguageServer.Framework.Protocol.Message.LinkedEditingRange;
using EmmyLua.LanguageServer.Framework.Protocol.Message.NotebookDocument;
using EmmyLua.LanguageServer.Framework.Protocol.Message.Progress;
using EmmyLua.LanguageServer.Framework.Protocol.Message.Reference;
using EmmyLua.LanguageServer.Framework.Protocol.Message.Rename;
using EmmyLua.LanguageServer.Framework.Protocol.Message.SelectionRange;
using EmmyLua.LanguageServer.Framework.Protocol.Message.SemanticToken;
using EmmyLua.LanguageServer.Framework.Protocol.Message.SignatureHelp;
using EmmyLua.LanguageServer.Framework.Protocol.Message.TextDocument;
using EmmyLua.LanguageServer.Framework.Protocol.Message.TypeDefinition;
using EmmyLua.LanguageServer.Framework.Protocol.Message.TypeHierarchy;
using EmmyLua.LanguageServer.Framework.Protocol.Message.WorkspaceDiagnostic;
using EmmyLua.LanguageServer.Framework.Protocol.Message.WorkspaceFiles;
using EmmyLua.LanguageServer.Framework.Protocol.Message.WorkspaceFolders;
using EmmyLua.LanguageServer.Framework.Protocol.Message.WorkspaceSymbol;
using EmmyLua.LanguageServer.Framework.Protocol.Message.WorkspaceWatchedFile;
using EmmyLua.LanguageServer.Framework.Protocol.Model;
using EmmyLua.LanguageServer.Framework.Protocol.Model.TextEdit;
using EmmyLua.LanguageServer.Framework.Protocol.Supplement;
using WCKYWCKF.LanguageClient.Union;

namespace WCKYWCKF.LanguageClient;

public static partial class LanguageClientEx
{
    // Auto-generated common methods (LSP 3.17 compliant)
    // Auto-generated request methods
        public static async Task<InitializeResult?> InitializeRequest(
        this LSPCommunicationBase lspCommunication,
        InitializeParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.initialize,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<InitializeResult>(result)
                .ConfigureAwait(false);
        }
        public static async Task RegisterCapabilityRequest(
        this LSPCommunicationBase lspCommunication,
        RegistrationParams @params,
        TimeSpan timeOut)
        {
            await lspCommunication.SendRequest(
                LSPDefaultMethod.client_registerCapability,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);
        }
        public static async Task UnregisterCapabilityRequest(
        this LSPCommunicationBase lspCommunication,
        UnregistrationParams @params,
        TimeSpan timeOut)
        {
            await lspCommunication.SendRequest(
                LSPDefaultMethod.client_unregisterCapability,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);
        }
        public static async Task ShutdownRequest(
        this LSPCommunicationBase lspCommunication,
        TimeSpan timeOut)
        {
            await lspCommunication.SendRequest(
                LSPDefaultMethod.shutdown,
                null,
                timeOut).ConfigureAwait(false);
        }
        public static async Task<List<TextEdit>?> WillSaveWaitUntilTextDocumentRequest(
        this LSPCommunicationBase lspCommunication,
        WillSaveTextDocumentParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.textDocument_willSaveWaitUntil,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<List<TextEdit>>(result)
                .ConfigureAwait(false);
        }
        public static async Task<LocationOrLocationListOrLocationLinkList?> GotoDeclarationRequest(
        this LSPCommunicationBase lspCommunication,
        DeclarationParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.textDocument_declaration,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<LocationOrLocationListOrLocationLinkList>(result)
                .ConfigureAwait(false);
        }
        public static async Task<LocationOrLocationListOrLocationLinkList?> GotoDefinitionRequest(
        this LSPCommunicationBase lspCommunication,
        DefinitionParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.textDocument_definition,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<LocationOrLocationListOrLocationLinkList>(result)
                .ConfigureAwait(false);
        }
        public static async Task<LocationOrLocationListOrLocationLinkList?> GotoTypeDefinitionRequest(
        this LSPCommunicationBase lspCommunication,
        TypeDefinitionParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.textDocument_typeDefinition,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<LocationOrLocationListOrLocationLinkList>(result)
                .ConfigureAwait(false);
        }
        public static async Task<LocationOrLocationListOrLocationLinkList?> GotoImplementationRequest(
        this LSPCommunicationBase lspCommunication,
        ImplementationParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.textDocument_implementation,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<LocationOrLocationListOrLocationLinkList>(result)
                .ConfigureAwait(false);
        }
        public static async Task<List<Location>?> FindReferencesRequest(
        this LSPCommunicationBase lspCommunication,
        ReferenceParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.textDocument_references,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<List<Location>>(result)
                .ConfigureAwait(false);
        }
        public static async Task<List<CallHierarchyItem>?> PrepareCallHierarchyRequest(
        this LSPCommunicationBase lspCommunication,
        CallHierarchyPrepareParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.textDocument_prepareCallHierarchy,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<List<CallHierarchyItem>>(result)
                .ConfigureAwait(false);
        }
        public static async Task<List<CallHierarchyIncomingCall>?> CallHierarchyIncomingCallsRequest(
        this LSPCommunicationBase lspCommunication,
        CallHierarchyIncomingCallsParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.callHierarchy_incomingCalls,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<List<CallHierarchyIncomingCall>>(result)
                .ConfigureAwait(false);
        }
        public static async Task<List<CallHierarchyOutgoingCall>?> CallHierarchyOutgoingCallsRequest(
        this LSPCommunicationBase lspCommunication,
        CallHierarchyOutgoingCallsParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.callHierarchy_outgoingCalls,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<List<CallHierarchyOutgoingCall>>(result)
                .ConfigureAwait(false);
        }
        public static async Task<List<TypeHierarchyItem>?> PrepareTypeHierarchyRequest(
        this LSPCommunicationBase lspCommunication,
        TypeHierarchyPrepareParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.textDocument_prepareTypeHierarchy,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<List<TypeHierarchyItem>>(result)
                .ConfigureAwait(false);
        }
        public static async Task<List<TypeHierarchyItem>?> TypeHierarchySupertypesRequest(
        this LSPCommunicationBase lspCommunication,
        TypeHierarchySupertypesParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.typeHierarchy_supertypes,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<List<TypeHierarchyItem>>(result)
                .ConfigureAwait(false);
        }
        public static async Task<List<TypeHierarchyItem>?> TypeHierarchySubtypesRequest(
        this LSPCommunicationBase lspCommunication,
        TypeHierarchySubtypesParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.typeHierarchy_subtypes,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<List<TypeHierarchyItem>>(result)
                .ConfigureAwait(false);
        }
        public static async Task<List<DocumentHighlight>?> DocumentHighlightsRequest(
        this LSPCommunicationBase lspCommunication,
        DocumentHighlightParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.textDocument_documentHighlight,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<List<DocumentHighlight>>(result)
                .ConfigureAwait(false);
        }
        public static async Task<List<DocumentLink>?> DocumentLinkRequest(
        this LSPCommunicationBase lspCommunication,
        DocumentLinkParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.textDocument_documentLink,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<List<DocumentLink>>(result)
                .ConfigureAwait(false);
        }
        public static async Task<DocumentLink?> DocumentLinkResolveRequest(
        this LSPCommunicationBase lspCommunication,
        DocumentLink @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.documentLink_resolve,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<DocumentLink>(result)
                .ConfigureAwait(false);
        }
        public static async Task<Hover?> HoverRequest(
        this LSPCommunicationBase lspCommunication,
        HoverParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.textDocument_hover,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<Hover>(result)
                .ConfigureAwait(false);
        }
        public static async Task<List<CodeLens>?> CodeLensRequest(
        this LSPCommunicationBase lspCommunication,
        CodeLensParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.textDocument_codeLens,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<List<CodeLens>>(result)
                .ConfigureAwait(false);
        }
        public static async Task<CodeLens?> CodeLensResolveRequest(
        this LSPCommunicationBase lspCommunication,
        CodeLens @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.codeLens_resolve,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<CodeLens>(result)
                .ConfigureAwait(false);
        }
        public static async Task<List<FoldingRange>?> FoldingRangeRequest(
        this LSPCommunicationBase lspCommunication,
        FoldingRangeParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.textDocument_foldingRange,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<List<FoldingRange>>(result)
                .ConfigureAwait(false);
        }
        public static async Task<List<SelectionRange>?> SelectionRangeRequest(
        this LSPCommunicationBase lspCommunication,
        SelectionRangeParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.textDocument_selectionRange,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<List<SelectionRange>>(result)
                .ConfigureAwait(false);
        }
        public static async Task<DocumentSymbolListOrSymbolInformationList?> DocumentSymbolsRequest(
        this LSPCommunicationBase lspCommunication,
        DocumentSymbolParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.textDocument_documentSymbol,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<DocumentSymbolListOrSymbolInformationList>(result)
                .ConfigureAwait(false);
        }
        public static async Task<SemanticTokens?> SemanticTokensForFullFileRequest(
        this LSPCommunicationBase lspCommunication,
        SemanticTokensParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.textDocument_semanticTokens_full,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<SemanticTokens>(result)
                .ConfigureAwait(false);
        }
        public static async Task<SemanticTokensOrSemanticTokensDelta?> SemanticTokensForDeltaFileRequest(
        this LSPCommunicationBase lspCommunication,
        SemanticTokensDeltaParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.textDocument_semanticTokens_full_delta,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<SemanticTokensOrSemanticTokensDelta>(result)
                .ConfigureAwait(false);
        }
        public static async Task<SemanticTokens?> SemanticTokensForRangeFileRequest(
        this LSPCommunicationBase lspCommunication,
        SemanticTokensRangeParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.textDocument_semanticTokens_range,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<SemanticTokens>(result)
                .ConfigureAwait(false);
        }
        public static async Task<List<InlayHint>?> InlayHintRequest(
        this LSPCommunicationBase lspCommunication,
        InlayHintParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.textDocument_inlayHint,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<List<InlayHint>>(result)
                .ConfigureAwait(false);
        }
        public static async Task<InlayHint?> InlayHintResolveRequest(
        this LSPCommunicationBase lspCommunication,
        InlayHint @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.inlayHint_resolve,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<InlayHint>(result)
                .ConfigureAwait(false);
        }
        public static async Task<List<InlineValue>?> InlineValueRequest(
        this LSPCommunicationBase lspCommunication,
        InlineValueParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.textDocument_inlineValue,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<List<InlineValue>>(result)
                .ConfigureAwait(false);
        }
        public static async Task<List<Moniker>?> MonikersRequest(
        this LSPCommunicationBase lspCommunication,
        MonikerParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.textDocument_moniker,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<List<Moniker>>(result)
                .ConfigureAwait(false);
        }
        public static async Task<CompletionItemListOrCompletionList?> CompletionRequest(
        this LSPCommunicationBase lspCommunication,
        CompletionParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.textDocument_completion,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<CompletionItemListOrCompletionList>(result)
                .ConfigureAwait(false);
        }
        public static async Task<CompletionItem?> CompletionItemResolveRequest(
        this LSPCommunicationBase lspCommunication,
        CompletionItem @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.completionItem_resolve,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<CompletionItem>(result)
                .ConfigureAwait(false);
        }
        public static async Task<DocumentDiagnosticReport?> DocumentDiagnosticsRequest(
        this LSPCommunicationBase lspCommunication,
        DocumentDiagnosticParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.textDocument_diagnostic,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<DocumentDiagnosticReport>(result)
                .ConfigureAwait(false);
        }
        public static async Task<WorkspaceDiagnosticReport?> WorkspaceDiagnosticsRequest(
        this LSPCommunicationBase lspCommunication,
        WorkspaceDiagnosticParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.workspace_diagnostic,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<WorkspaceDiagnosticReport>(result)
                .ConfigureAwait(false);
        }
        public static async Task<SignatureHelp?> SignatureHelpRequest(
        this LSPCommunicationBase lspCommunication,
        SignatureHelpParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.textDocument_signatureHelp,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<SignatureHelp>(result)
                .ConfigureAwait(false);
        }
        public static async Task<List<CommandOrCodeAction>?> CodeActionRequest(
        this LSPCommunicationBase lspCommunication,
        CodeActionParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.textDocument_codeAction,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<List<CommandOrCodeAction>>(result)
                .ConfigureAwait(false);
        }
        public static async Task<CodeAction?> CodeActionResolveRequest(
        this LSPCommunicationBase lspCommunication,
        CodeAction @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.codeAction_resolve,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<CodeAction>(result)
                .ConfigureAwait(false);
        }
        public static async Task<List<ColorInformation>?> DocumentColorRequest(
        this LSPCommunicationBase lspCommunication,
        DocumentColorParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.textDocument_documentColor,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<List<ColorInformation>>(result)
                .ConfigureAwait(false);
        }
        public static async Task<List<ColorPresentation>?> ColorPresentationRequest(
        this LSPCommunicationBase lspCommunication,
        ColorPresentationParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.textDocument_colorPresentation,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<List<ColorPresentation>>(result)
                .ConfigureAwait(false);
        }
        public static async Task<List<TextEdit>?> DocumentFormattingRequest(
        this LSPCommunicationBase lspCommunication,
        DocumentFormattingParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.textDocument_formatting,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<List<TextEdit>>(result)
                .ConfigureAwait(false);
        }
        public static async Task<List<TextEdit>?> DocumentRangeFormattingRequest(
        this LSPCommunicationBase lspCommunication,
        DocumentRangeFormattingParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.textDocument_rangeFormatting,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<List<TextEdit>>(result)
                .ConfigureAwait(false);
        }
        public static async Task<List<TextEdit>?> DocumentonTypeFormattingRequest(
        this LSPCommunicationBase lspCommunication,
        DocumentOnTypeFormattingParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.textDocument_onTypeFormatting,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<List<TextEdit>>(result)
                .ConfigureAwait(false);
        }
        public static async Task<WorkspaceEdit?> RenameRequest(
        this LSPCommunicationBase lspCommunication,
        RenameParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.textDocument_rename,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<WorkspaceEdit>(result)
                .ConfigureAwait(false);
        }
        public static async Task<RangeOrPrepareRenameResult?> PrepareRenameRequest(
        this LSPCommunicationBase lspCommunication,
        PrepareRenameParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.textDocument_prepareRename,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<RangeOrPrepareRenameResult>(result)
                .ConfigureAwait(false);
        }
        public static async Task<LinkedEditingRanges?> LinkedEditingRangeRequest(
        this LSPCommunicationBase lspCommunication,
        LinkedEditingRangeParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.textDocument_linkedEditingRange,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<LinkedEditingRanges>(result)
                .ConfigureAwait(false);
        }
        public static async Task<InlineCompletionItemListOrInlineCompletionList?> InlineCompletionRequest(
        this LSPCommunicationBase lspCommunication,
        InlineCompletionParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.textDocument_inlineCompletion,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<InlineCompletionItemListOrInlineCompletionList>(result)
                .ConfigureAwait(false);
        }
        public static async Task<SymbolInformationListOrWorkspaceSymbolList?> WorkspaceSymbolsRequest(
        this LSPCommunicationBase lspCommunication,
        WorkspaceSymbolParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.workspace_symbol,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<SymbolInformationListOrWorkspaceSymbolList>(result)
                .ConfigureAwait(false);
        }
        public static async Task<WorkspaceSymbol?> WorkspaceSymbolResolveRequest(
        this LSPCommunicationBase lspCommunication,
        WorkspaceSymbol @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.workspaceSymbol_resolve,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<WorkspaceSymbol>(result)
                .ConfigureAwait(false);
        }
        public static async Task<List<LSPAny>?> ConfigurationRequest(
        this LSPCommunicationBase lspCommunication,
        ConfigurationParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.workspace_configuration,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<List<LSPAny>>(result)
                .ConfigureAwait(false);
        }
        public static async Task<List<WorkspaceFolder>?> WorkspaceFoldersRequest(
        this LSPCommunicationBase lspCommunication,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.workspace_workspaceFolders,
                null,
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<List<WorkspaceFolder>>(result)
                .ConfigureAwait(false);
        }
        public static async Task<WorkspaceEdit?> WillCreateFilesRequest(
        this LSPCommunicationBase lspCommunication,
        CreateFilesParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.workspace_willCreateFiles,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<WorkspaceEdit>(result)
                .ConfigureAwait(false);
        }
        public static async Task<WorkspaceEdit?> WillRenameFilesRequest(
        this LSPCommunicationBase lspCommunication,
        RenameFilesParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.workspace_willRenameFiles,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<WorkspaceEdit>(result)
                .ConfigureAwait(false);
        }
        public static async Task<WorkspaceEdit?> WillDeleteFilesRequest(
        this LSPCommunicationBase lspCommunication,
        DeleteFilesParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.workspace_willDeleteFiles,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<WorkspaceEdit>(result)
                .ConfigureAwait(false);
        }
        public static async Task<LSPAny?> ExecuteacommandRequest(
        this LSPCommunicationBase lspCommunication,
        ExecuteCommandParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.workspace_executeCommand,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<LSPAny>(result)
                .ConfigureAwait(false);
        }
        public static async Task<ApplyWorkspaceEditResult?> AppliesaWorkspaceEditRequest(
        this LSPCommunicationBase lspCommunication,
        ApplyWorkspaceEditParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.workspace_applyEdit,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<ApplyWorkspaceEditResult>(result)
                .ConfigureAwait(false);
        }
        public static async Task<TextDocumentContentResult?> TextDocumentContentRequest(
        this LSPCommunicationBase lspCommunication,
        TextDocumentContentParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.workspace_textDocumentContent,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<TextDocumentContentResult>(result)
                .ConfigureAwait(false);
        }
        public static async Task TextDocumentContentRefreshRequest(
        this LSPCommunicationBase lspCommunication,
        TextDocumentContentRefreshParams @params,
        TimeSpan timeOut)
        {
            await lspCommunication.SendRequest(
                LSPDefaultMethod.workspace_textDocumentContent_refresh,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);
        }
        public static async Task<MessageActionItem?> ShowMessageRequest(
        this LSPCommunicationBase lspCommunication,
        ShowMessageRequestParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.window_showMessageRequest,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<MessageActionItem>(result)
                .ConfigureAwait(false);
        }
        public static async Task<ShowDocumentResult?> ShowDocumentRequest(
        this LSPCommunicationBase lspCommunication,
        ShowDocumentParams @params,
        TimeSpan timeOut)
        {
            var result = await lspCommunication.SendRequest(
                LSPDefaultMethod.window_showDocument,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);

            return await lspCommunication.DeserializeAsync<ShowDocumentResult>(result)
                .ConfigureAwait(false);
        }
        public static async Task CreateWorkDoneProgressRequest(
        this LSPCommunicationBase lspCommunication,
        WorkDoneProgressCreateParams @params,
        TimeSpan timeOut)
        {
            await lspCommunication.SendRequest(
                LSPDefaultMethod.window_workDoneProgress_create,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false),
                timeOut).ConfigureAwait(false);
        }

    // Auto-generated notification methods
    public static async Task InitializedNotification(
        this LSPCommunicationBase lspCommunication,
        InitializedParams @params)
    {
        await lspCommunication.SendNotification(
            new NotificationMessage(
                LSPDefaultMethod.initialized,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false)));
    }
    public static async Task SetTraceNotification(
        this LSPCommunicationBase lspCommunication,
        SetTraceParams @params)
    {
        await lspCommunication.SendNotification(
            new NotificationMessage(
                LSPDefaultMethod.setTrace,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false)));
    }
    public static async Task LogTraceNotification(
        this LSPCommunicationBase lspCommunication,
        LogTraceParams @params)
    {
        await lspCommunication.SendNotification(
            new NotificationMessage(
                LSPDefaultMethod.logTrace,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false)));
    }
    public static async Task ExitNotification(
        this LSPCommunicationBase lspCommunication)
    {
        await lspCommunication.SendNotification(
            new NotificationMessage(
                LSPDefaultMethod.exit,
                null));
    }
    public static async Task DidOpenTextDocumentNotification(
        this LSPCommunicationBase lspCommunication,
        DidOpenTextDocumentParams @params)
    {
        await lspCommunication.SendNotification(
            new NotificationMessage(
                LSPDefaultMethod.textDocument_didOpen,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false)));
    }
    public static async Task DidChangeTextDocumentNotification(
        this LSPCommunicationBase lspCommunication,
        DidChangeTextDocumentParams @params)
    {
        await lspCommunication.SendNotification(
            new NotificationMessage(
                LSPDefaultMethod.textDocument_didChange,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false)));
    }
    public static async Task WillSaveTextDocumentNotification(
        this LSPCommunicationBase lspCommunication,
        WillSaveTextDocumentParams @params)
    {
        await lspCommunication.SendNotification(
            new NotificationMessage(
                LSPDefaultMethod.textDocument_willSave,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false)));
    }
    public static async Task DidSaveTextDocumentNotification(
        this LSPCommunicationBase lspCommunication,
        DidSaveTextDocumentParams @params)
    {
        await lspCommunication.SendNotification(
            new NotificationMessage(
                LSPDefaultMethod.textDocument_didSave,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false)));
    }
    public static async Task DidCloseTextDocumentNotification(
        this LSPCommunicationBase lspCommunication,
        DidCloseTextDocumentParams @params)
    {
        await lspCommunication.SendNotification(
            new NotificationMessage(
                LSPDefaultMethod.textDocument_didClose,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false)));
    }
    public static async Task DidOpenNotebookDocumentNotification(
        this LSPCommunicationBase lspCommunication,
        DidOpenNotebookDocumentParams @params)
    {
        await lspCommunication.SendNotification(
            new NotificationMessage(
                LSPDefaultMethod.notebookDocument_didOpen,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false)));
    }
    public static async Task DidChangeNotebookDocumentNotification(
        this LSPCommunicationBase lspCommunication,
        DidChangeNotebookDocumentParams @params)
    {
        await lspCommunication.SendNotification(
            new NotificationMessage(
                LSPDefaultMethod.notebookDocument_didChange,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false)));
    }
    public static async Task DidSaveNotebookDocumentNotification(
        this LSPCommunicationBase lspCommunication,
        DidSaveNotebookDocumentParams @params)
    {
        await lspCommunication.SendNotification(
            new NotificationMessage(
                LSPDefaultMethod.notebookDocument_didSave,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false)));
    }
    public static async Task DidCloseNotebookDocumentNotification(
        this LSPCommunicationBase lspCommunication,
        DidCloseNotebookDocumentParams @params)
    {
        await lspCommunication.SendNotification(
            new NotificationMessage(
                LSPDefaultMethod.notebookDocument_didClose,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false)));
    }
    public static async Task CodeLensRefreshRequestNotification(
        this LSPCommunicationBase lspCommunication)
    {
        await lspCommunication.SendNotification(
            new NotificationMessage(
                LSPDefaultMethod.workspace_codeLens_refresh,
                null));
    }
    public static async Task FoldingRangeRefreshRequestNotification(
        this LSPCommunicationBase lspCommunication)
    {
        await lspCommunication.SendNotification(
            new NotificationMessage(
                LSPDefaultMethod.workspace_foldingRange_refresh,
                null));
    }
    public static async Task SemanticTokensForRefreshFileNotification(
        this LSPCommunicationBase lspCommunication)
    {
        await lspCommunication.SendNotification(
            new NotificationMessage(
                LSPDefaultMethod.workspace_semanticTokens_refresh,
                null));
    }
    public static async Task InlayHintRefreshRequestNotification(
        this LSPCommunicationBase lspCommunication)
    {
        await lspCommunication.SendNotification(
            new NotificationMessage(
                LSPDefaultMethod.workspace_inlayHint_refresh,
                null));
    }
    public static async Task InlineValueRefreshRequestNotification(
        this LSPCommunicationBase lspCommunication)
    {
        await lspCommunication.SendNotification(
            new NotificationMessage(
                LSPDefaultMethod.workspace_inlineValue_refresh,
                null));
    }
    public static async Task PublishDiagnosticsNotification(
        this LSPCommunicationBase lspCommunication,
        PublishDiagnosticsParams @params)
    {
        await lspCommunication.SendNotification(
            new NotificationMessage(
                LSPDefaultMethod.textDocument_publishDiagnostics,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false)));
    }
    public static async Task DiagnosticsRefreshNotification(
        this LSPCommunicationBase lspCommunication)
    {
        await lspCommunication.SendNotification(
            new NotificationMessage(
                LSPDefaultMethod.workspace_diagnostic_refresh,
                null));
    }
    public static async Task DidChangeConfigurationNotification(
        this LSPCommunicationBase lspCommunication,
        DidChangeConfigurationParams @params)
    {
        await lspCommunication.SendNotification(
            new NotificationMessage(
                LSPDefaultMethod.workspace_didChangeConfiguration,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false)));
    }
    public static async Task DidChangeWorkspaceFoldersNotification(
        this LSPCommunicationBase lspCommunication,
        DidChangeWorkspaceFoldersParams @params)
    {
        await lspCommunication.SendNotification(
            new NotificationMessage(
                LSPDefaultMethod.workspace_didChangeWorkspaceFolders,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false)));
    }
    public static async Task DidCreateFilesNotification(
        this LSPCommunicationBase lspCommunication,
        CreateFilesParams @params)
    {
        await lspCommunication.SendNotification(
            new NotificationMessage(
                LSPDefaultMethod.workspace_didCreateFiles,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false)));
    }
    public static async Task DidRenameFilesNotification(
        this LSPCommunicationBase lspCommunication,
        RenameFilesParams @params)
    {
        await lspCommunication.SendNotification(
            new NotificationMessage(
                LSPDefaultMethod.workspace_didRenameFiles,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false)));
    }
    public static async Task DidDeleteFilesNotification(
        this LSPCommunicationBase lspCommunication,
        DeleteFilesParams @params)
    {
        await lspCommunication.SendNotification(
            new NotificationMessage(
                LSPDefaultMethod.workspace_didDeleteFiles,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false)));
    }
    public static async Task DidChangeWatchedFilesNotification(
        this LSPCommunicationBase lspCommunication,
        DidChangeWatchedFilesParams @params)
    {
        await lspCommunication.SendNotification(
            new NotificationMessage(
                LSPDefaultMethod.workspace_didChangeWatchedFiles,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false)));
    }
    public static async Task ShowMessageNotification(
        this LSPCommunicationBase lspCommunication,
        ShowMessageParams @params)
    {
        await lspCommunication.SendNotification(
            new NotificationMessage(
                LSPDefaultMethod.window_showMessage,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false)));
    }
    public static async Task LogMessageNotification(
        this LSPCommunicationBase lspCommunication,
        LogMessageParams @params)
    {
        await lspCommunication.SendNotification(
            new NotificationMessage(
                LSPDefaultMethod.window_logMessage,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false)));
    }
    public static async Task CancelaWorkDoneProgressNotification(
        this LSPCommunicationBase lspCommunication,
        WorkDoneProgressCancelParams @params)
    {
        await lspCommunication.SendNotification(
            new NotificationMessage(
                LSPDefaultMethod.window_workDoneProgress_cancel,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false)));
    }
    public static async Task TelemetryNotification(
        this LSPCommunicationBase lspCommunication,
        (object,Array) @params)
    {
        await lspCommunication.SendNotification(
            new NotificationMessage(
                LSPDefaultMethod.telemetry_event,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false)));
    }
    public static async Task ProgressSupportNotification(
        this LSPCommunicationBase lspCommunication,
        ProgressParams @params)
    {
        await lspCommunication.SendNotification(
            new NotificationMessage(
                LSPDefaultMethod.progress,
                await lspCommunication.SerializeToDocumentAsync(@params).ConfigureAwait(false)));
    }
}

