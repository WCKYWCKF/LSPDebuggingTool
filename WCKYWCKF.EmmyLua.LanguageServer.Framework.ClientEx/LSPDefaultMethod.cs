namespace WCKYWCKF.EmmyLua.LanguageServer.Framework.ClientEx;

public struct LSPDefaultMethod
{
    // 生命周期方法
    public const string initialize = "initialize";
    public const string initialized = "initialized";
    public const string shutdown = "shutdown";
    public const string exit = "exit";

    // 文本文档同步
    public const string textDocument_didOpen = "textDocument/didOpen";
    public const string textDocument_didChange = "textDocument/didChange";
    public const string textDocument_didClose = "textDocument/didClose";
    public const string textDocument_willSave = "textDocument/willSave";
    public const string textDocument_willSaveWaitUntil = "textDocument/willSaveWaitUntil";
    public const string textDocument_didSave = "textDocument/didSave";

    // 语言功能
    public const string textDocument_completion = "textDocument/completion";
    public const string textDocument_hover = "textDocument/hover";
    public const string textDocument_signatureHelp = "textDocument/signatureHelp";
    public const string textDocument_definition = "textDocument/definition";
    public const string textDocument_references = "textDocument/references";
    public const string textDocument_rename = "textDocument/rename";
    public const string textDocument_codeAction = "textDocument/codeAction";
    public const string textDocument_formatting = "textDocument/formatting";
    public const string textDocument_documentHighlight = "textDocument/documentHighlight";
    public const string textDocument_typeDefinition = "textDocument/typeDefinition";
    public const string textDocument_implementation = "textDocument/implementation";
    public const string textDocument_declaration = "textDocument/declaration";

    // 符号和结构
    public const string textDocument_documentLink = "textDocument/documentLink";
    public const string textDocument_documentSymbol = "textDocument/documentSymbol";
    public const string textDocument_foldingRange = "textDocument/foldingRange";
    public const string textDocument_codeLens = "textDocument/codeLens";
    public const string textDocument_selectionRange = "textDocument/selectionRange";

    // 语义令牌
    public const string textDocument_semanticTokens_full = "textDocument/semanticTokens/full";
    public const string textDocument_semanticTokens_full_delta = "textDocument/semanticTokens/full/delta";
    public const string textDocument_semanticTokens_range = "textDocument/semanticTokens/range";

    // 层次结构
    public const string textDocument_prepareCallHierarchy = "textDocument/prepareCallHierarchy";
    public const string callHierarchy_incomingCalls = "callHierarchy/incomingCalls";
    public const string callHierarchy_outgoingCalls = "callHierarchy/outgoingCalls";

    // 工作区相关
    public const string workspace_didChangeConfiguration = "workspace/didChangeConfiguration";
    public const string workspace_executeCommand = "workspace/executeCommand";
    public const string workspace_symbol = "workspace/symbol";
    public const string workspace_didChangeWatchedFiles = "workspace/didChangeWatchedFiles";

    // 窗口相关
    public const string window_showMessage = "window/showMessage";
    public const string window_showDocument = "window/showDocument";
    public const string window_workDoneProgress_create = "window/workDoneProgress/create";

    // 诊断
    public const string textDocument_diagnostic = "textDocument/diagnostic";
    public const string workspace_diagnostic = "workspace/diagnostic";
}