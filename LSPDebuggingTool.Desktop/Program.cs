using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.Reactive.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
// using Akavache;
using Avalonia;
using Avalonia.Dialogs;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using AvaloniaEdit.Document;
using AvaloniaEditLSPIntegration;
using EmmyLua.LanguageServer.Framework.Protocol.Capabilities.Client.ClientCapabilities;
using EmmyLua.LanguageServer.Framework.Protocol.Capabilities.Client.TextDocumentClientCapabilities;
using EmmyLua.LanguageServer.Framework.Protocol.Capabilities.Client.WorkspaceEditClientCapabilities;
using EmmyLua.LanguageServer.Framework.Protocol.JsonRpc;
using EmmyLua.LanguageServer.Framework.Protocol.Message.DocumentSymbol;
using EmmyLua.LanguageServer.Framework.Protocol.Message.Initialize;
using EmmyLua.LanguageServer.Framework.Protocol.Message.SemanticToken;
using EmmyLua.LanguageServer.Framework.Protocol.Message.TextDocument;
using EmmyLua.LanguageServer.Framework.Protocol.Model;
using EmmyLua.LanguageServer.Framework.Protocol.Model.TextDocument;
using EmmyLua.LanguageServer.Framework.Server;
using WCKYWCKF.EmmyLua.LanguageServer.Framework.ClientEx;

namespace LSPDebuggingTool.Desktop;

[SupportedOSPlatform("windows")]
[SupportedOSPlatform("linux")]
[SupportedOSPlatform("macOS")]
internal sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
//         var process = Process.Start(new ProcessStartInfo()
//         {
//             FileName = @"D:\Temp\tinymist.exe",
//             ArgumentList =
//             {
//                 "lsp",
//                 "--mirror",
//                 "D:\\Project\\Typst LSP test\\tinymist-lsp.txt"
//             },
//             CreateNoWindow = true,
//             RedirectStandardOutput = true,
//             RedirectStandardInput = true,
//             RedirectStandardError = true
//         }) ?? throw new InvalidOperationException();
// #pragma warning disable VSTHRD110
//         Task.Run(() =>
//         {
//             while (process.HasExited == false)
//             {
//                 Console.WriteLine(process.StandardError.ReadLine());
//             }
//
//             Console.WriteLine("Process exited");
//         });
// #pragma warning restore VSTHRD110
//         PipeWriter writer = PipeWriter.Create(process.StandardInput.BaseStream);
//         var languageServer = LanguageServer.From(process.StandardOutput.BaseStream, writer.AsStream());
// //         writer.WriteAsync("""
// //                           Content-Length: 6794
// //
// //                           {"jsonrpc":"2.0","id":0,"method":"initialize","params":{"processId":26236,"clientInfo":{"name":"Visual Studio Code","version":"1.96.4"},"locale":"zh-cn","rootPath":"d:\\Project\\Typst LSP test","rootUri":"file:///d%3A/Project/Typst%20LSP%20test","capabilities":{"workspace":{"applyEdit":true,"workspaceEdit":{"documentChanges":true,"resourceOperations":["create","rename","delete"],"failureHandling":"textOnlyTransactional","normalizesLineEndings":true,"changeAnnotationSupport":{"groupsOnLabel":true}},"configuration":true,"didChangeWatchedFiles":{"dynamicRegistration":true,"relativePatternSupport":true},"symbol":{"dynamicRegistration":true,"symbolKind":{"valueSet":[1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26]},"tagSupport":{"valueSet":[1]},"resolveSupport":{"properties":["location.range"]}},"codeLens":{"refreshSupport":true},"executeCommand":{"dynamicRegistration":true},"didChangeConfiguration":{"dynamicRegistration":true},"workspaceFolders":true,"foldingRange":{"refreshSupport":true},"semanticTokens":{"refreshSupport":true},"fileOperations":{"dynamicRegistration":true,"didCreate":true,"didRename":true,"didDelete":true,"willCreate":true,"willRename":true,"willDelete":true},"inlineValue":{"refreshSupport":true},"inlayHint":{"refreshSupport":true},"diagnostics":{"refreshSupport":true}},"textDocument":{"publishDiagnostics":{"relatedInformation":true,"versionSupport":false,"tagSupport":{"valueSet":[1,2]},"codeDescriptionSupport":true,"dataSupport":true},"synchronization":{"dynamicRegistration":true,"willSave":true,"willSaveWaitUntil":true,"didSave":true},"completion":{"dynamicRegistration":true,"contextSupport":true,"completionItem":{"snippetSupport":true,"commitCharactersSupport":true,"documentationFormat":["markdown","plaintext"],"deprecatedSupport":true,"preselectSupport":true,"tagSupport":{"valueSet":[1]},"insertReplaceSupport":true,"resolveSupport":{"properties":["documentation","detail","additionalTextEdits"]},"insertTextModeSupport":{"valueSet":[1,2]},"labelDetailsSupport":true},"insertTextMode":2,"completionItemKind":{"valueSet":[1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25]},"completionList":{"itemDefaults":["commitCharacters","editRange","insertTextFormat","insertTextMode","data"]}},"hover":{"dynamicRegistration":true,"contentFormat":["markdown","plaintext"]},"signatureHelp":{"dynamicRegistration":true,"signatureInformation":{"documentationFormat":["markdown","plaintext"],"parameterInformation":{"labelOffsetSupport":true},"activeParameterSupport":true},"contextSupport":true},"definition":{"dynamicRegistration":true,"linkSupport":true},"references":{"dynamicRegistration":true},"documentHighlight":{"dynamicRegistration":true},"documentSymbol":{"dynamicRegistration":true,"symbolKind":{"valueSet":[1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26]},"hierarchicalDocumentSymbolSupport":true,"tagSupport":{"valueSet":[1]},"labelSupport":true},"codeAction":{"dynamicRegistration":true,"isPreferredSupport":true,"disabledSupport":true,"dataSupport":true,"resolveSupport":{"properties":["edit"]},"codeActionLiteralSupport":{"codeActionKind":{"valueSet":["","quickfix","refactor","refactor.extract","refactor.inline","refactor.rewrite","source","source.organizeImports"]}},"honorsChangeAnnotations":true},"codeLens":{"dynamicRegistration":true},"formatting":{"dynamicRegistration":true},"rangeFormatting":{"dynamicRegistration":true,"rangesSupport":true},"onTypeFormatting":{"dynamicRegistration":true},"rename":{"dynamicRegistration":true,"prepareSupport":true,"prepareSupportDefaultBehavior":1,"honorsChangeAnnotations":true},"documentLink":{"dynamicRegistration":true,"tooltipSupport":true},"typeDefinition":{"dynamicRegistration":true,"linkSupport":true},"implementation":{"dynamicRegistration":true,"linkSupport":true},"colorProvider":{"dynamicRegistration":true},"foldingRange":{"dynamicRegistration":true,"rangeLimit":5000,"lineFoldingOnly":true,"foldingRangeKind":{"valueSet":["comment","imports","region"]},"foldingRange":{"collapsedText":false}},"declaration":{"dynamicRegistration":true,"linkSupport":true},"selectionRange":{"dynamicRegistration":true},"callHierarchy":{"dynamicRegistration":true},"semanticTokens":{"dynamicRegistration":true,"tokenTypes":["namespace","type","class","enum","interface","struct","typeParameter","parameter","variable","property","enumMember","event","function","method","macro","keyword","modifier","comment","string","number","regexp","operator","decorator"],"tokenModifiers":["declaration","definition","readonly","static","deprecated","abstract","async","modification","documentation","defaultLibrary"],"formats":["relative"],"requests":{"range":true,"full":{"delta":true}},"multilineTokenSupport":false,"overlappingTokenSupport":false,"serverCancelSupport":true,"augmentsSyntaxTokens":true},"linkedEditingRange":{"dynamicRegistration":true},"typeHierarchy":{"dynamicRegistration":true},"inlineValue":{"dynamicRegistration":true},"inlayHint":{"dynamicRegistration":true,"resolveSupport":{"properties":["tooltip","textEdits","label.tooltip","label.location","label.command"]}},"diagnostic":{"dynamicRegistration":true,"relatedDocumentSupport":false}},"window":{"showMessage":{"messageActionItem":{"additionalPropertiesSupport":true}},"showDocument":{"support":true},"workDoneProgress":true},"general":{"staleRequestSupport":{"cancel":true,"retryOnContentModified":["textDocument/semanticTokens/full","textDocument/semanticTokens/range","textDocument/semanticTokens/full/delta"]},"regularExpressions":{"engine":"ECMAScript","version":"ES2020"},"markdown":{"parser":"marked","version":"1.1.0"},"positionEncodings":["utf-16"]},"notebookDocument":{"synchronization":{"dynamicRegistration":true,"executionSummarySupport":true}}},"initializationOptions":{"outputPath":"","exportPdf":"never","configureDefaultWordSeparator":"enable","semanticTokens":"enable","typingContinueCommentsOnNewline":true,"onEnterEvent":true,"systemFonts":true,"compileStatus":"enable","typstExtraArgs":[],"trace":{"server":"off"},"formatterMode":"disable","formatterPrintWidth":120,"showExportFileIn":null,"dragAndDrop":"enable","renderDocs":"enable","completion":{"triggerOnSnippetPlaceholders":false,"postfix":true,"postfixUfcs":true,"postfixUfcsLeft":true,"postfixUfcsRight":true},"previewFeature":"enable","preview":{"sysInputs":{},"systemFonts":true,"fontPaths":[],"refresh":"onType","scrollSync":"onSelectionChangeByMouse","partialRendering":true,"invertColors":null,"cursorIndicator":false,"pinPreviewFile":false},"colorTheme":"dark","triggerSuggest":true,"triggerSuggestAndParameterHints":true,"triggerParameterHints":true,"supportHtmlInMarkdown":true},"trace":"off","workspaceFolders":[{"uri":"file:///d%3A/Project/Typst%20LSP%20test","name":"Typst LSP test"}]}}Content-Length: 52
// //
// //                           {"jsonrpc":"2.0","method":"initialized","params":{}}Content-Length: 1059
// //
// //                           {"jsonrpc":"2.0","method":"textDocument/didOpen","params":{"textDocument":{"uri":"file:///d%3A/Project/Typst%20LSP%20test/temp.json","languageId":"json","version":1,"text":"{\r\n    \"processId\": null,\r\n    \"clientInfo\": {\r\n        \"name\": \"LSPDebuggingTool.Desktop, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\",\r\n        \"version\": \"1.0.0+8ed6bf1117f43f6e1d588644f4c4553a9ecb9f01\"\r\n    },\r\n    \"rootPath\": \"d:\\\\Project\\\\Typst LSP test\",\r\n    \"rootUri\": \"file:///d:/Project/Typst%20LSP%20test\",\r\n    \"initializationOptions\": null,\r\n    \"capabilities\": {\r\n        \"workspace\": {\r\n            \"workspaceFolders\": true\r\n        },\r\n        \"textDocument\": {},\r\n        \"window\": {\r\n            \"workDoneProgress\": true\r\n        },\r\n        \"general\": {},\r\n        \"experimental\": {}\r\n    },\r\n    \"workspaceFolders\": [\r\n        {\r\n            \"uri\": \"file:///d:/Project/Typst%20LSP%20test\",\r\n            \"name\": \"D:\\\\Project\"\r\n        }\r\n    ]\r\n}"}}}Content-Length: 49
// //
// //                           {"jsonrpc":"2.0","method":"shutdown","params":{}}Content-Length: 45
// //
// //                           {"jsonrpc":"2.0","method":"exit","params":{}}
// //                           """u8.ToArray()).AsTask().Wait();
//         // while (process.HasExited is false)
//         // {
//         // }
//         languageServer.AddJsonSerializeContext(JsonProtocolContext.Default);
//         languageServer.Run();
//         var initializeParams = new InitializeParams()
//         {
//             ProcessId = null,
//             ClientInfo = new ClientInfo()
//             {
//                 Name = "Typst LSP test",
//                 Version = "1.0.0"
//             },
//             Locale = "zh-cn",
//             RootUri = new DocumentUri(new Uri(@"D:\Project\Typst LSP test")),
//             RootPath = @"D:\Project\Typst LSP test",
//             WorkspaceFolders =
//             [
//                 new WorkspaceFolder(new DocumentUri(new Uri(@"D:\Project\Typst LSP test")), "Typst LSP test")
//             ],
//             Trace = TraceValue.Off,
//             Capabilities = new ClientCapabilities()
//             {
//                 Workspace = new WorkspaceClientCapabilities()
//                 {
//                     WorkspaceFolders = true
//                 },
//                 TextDocument = new TextDocumentClientCapabilities(),
//                 Window = new WindowClientCapabilities()
//                 {
//                     WorkDoneProgress = true
//                 },
//                 General = new GeneralClientCapabilities(),
//                 Experimental = JsonDocument.Parse("{}")
//             },
//             InitializationOptions = JsonDocument.Parse("{}")
//         };
//         var serverC = languageServer.SendInitializeRequest(initializeParams, TimeSpan.FromSeconds(5)).Result;
//         languageServer.SendSendInitializedNotification().Wait();
//         languageServer.SendDidOpenTextDocumentNotification(new DidOpenTextDocumentParams()
//         {
//             TextDocument = new TextDocumentItem()
//             {
//                 Uri = new DocumentUri(new Uri(@"D:\Project\Typst LSP test\main.typ")),
//                 LanguageId = "typst",
//                 Version = 0,
//                 Text = File.ReadAllText(@"D:\Project\Typst LSP test\main.typ")
//             }
//         }).Wait();
//         var semanticTokens = languageServer.SendSemanticTokensForFullRequest(new SemanticTokensParams()
//         {
//             TextDocument = new TextDocumentIdentifier(new DocumentUri(new Uri(@"D:\Project\Typst LSP test\main.typ")))
//         }, TimeSpan.FromSeconds(2)).Result;
//
//         languageServer.SendDidChangeTextDocumentNotification(new DidChangeTextDocumentParams()
//         {
//             TextDocument = new VersionedTextDocumentIdentifier(
//                 new DocumentUri(new Uri(@"D:\Project\Typst LSP test\main.typ"))
//                 , 1
//             ),
//             ContentChanges =
//             [
//                 new TextDocumentContentChangeEvent()
//                 {
//                     Range = new DocumentRange(new Position(39, 0), new Position(39, 0)),
//                     RangeLength = 0,
//                     Text = "#point[\n  #lorem(10).\n]"
//                 }
//             ]
//         }).Wait();
//
//         var semanticTokensDelta = languageServer.SendSemanticTokensForDeltaRequest(new SemanticTokensDeltaParams()
//         {
//             TextDocument = new TextDocumentIdentifier(new DocumentUri(new Uri(@"D:\Project\Typst LSP test\main.typ"))),
//             PreviousResultId = semanticTokens?.ResultId
//         }, TimeSpan.FromSeconds(2)).Result;
//
//         return;
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .UseManagedSystemDialogs()
            .WithInterFont()
            .UseReactiveUI()
            .LogToTrace();
    }
}

[JsonSerializable(typeof(SemanticTokensDeltaParams))]
[JsonSerializable(typeof(SemanticTokensDelta))]
public partial class JsonProtocolContext : JsonSerializerContext;