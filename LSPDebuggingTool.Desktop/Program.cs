using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Text.Json;
using Avalonia;
using Avalonia.Dialogs;
using Avalonia.ReactiveUI;
using EmmyLua.LanguageServer.Framework.Protocol;
using EmmyLua.LanguageServer.Framework.Protocol.Model.Markup;
using WCKYWCKF.EmmyLua.LanguageServer.Framework.ClientEx;

// using Akavache;

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
        // JsonSerializerOptions options = new();
        // options.TypeInfoResolverChain.Add(ClientExJSC.Default);
        // options.TypeInfoResolverChain.Add(JsonProtocolContext.Default);
        //
        // var json = JsonSerializer.Serialize(
        //     new MarkedStringsOrMarkupContent()
        //         { MarkupContent = new MarkupContent() { Value = "sdf", Kind = MarkupKind.Markdown } }, options);
        // var markedStrings = JsonSerializer.Deserialize<MarkedStringsOrMarkupContent>(json, options);
        // json = JsonSerializer.Serialize(
        //     new MarkedStringsOrMarkupContent()
        //         { MarkedStrings = [new MarkedString() { Language = "sdfsdf", Value = "sldfkjsdlfk" }] }, options);
        //  markedStrings = JsonSerializer.Deserialize<MarkedStringsOrMarkupContent>(json, options);
        // json = JsonSerializer.Serialize(
        //     new MarkedStringsOrMarkupContent()
        //     {
        //         MarkedStrings =
        //         [
        //             new MarkedString() { Language = "sdfsdf", Value = "sldfkjsdlfk" },
        //             new MarkedString() { Language = "12", Value = "4444444" }
        //         ]
        //     }, options);
        //  markedStrings = JsonSerializer.Deserialize<MarkedStringsOrMarkupContent>(json, options);
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