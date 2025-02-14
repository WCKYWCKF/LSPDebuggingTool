using System;
using System.IO;
using Serilog;

namespace LSPDebuggingTool.Models;

public static class RuntimeBasicInfo
{
    static RuntimeBasicInfo()
    {
        Log = Serilog.Log.Logger =
            new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(Path.Combine(LogPath, "log.txt"), rollingInterval: RollingInterval.Day)
                .CreateLogger();
        Log.Information("运行时基础信息已初始化");
    }

    public static string BasicPath { get; } =
        CheckPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            nameof(LSPDebuggingTool)));

    public static string LogPath { get; } =
        CheckPath(Path.Combine(BasicPath, "logs"));

    public static string AncillaryInformationPath { get; } = CheckPath(Path.Combine(BasicPath, "AncillaryInformation"));

    public static string SettingPath { get; } = Path.Combine(BasicPath, "Settings.json");

    public static ILogger Log { get; }


    private static string CheckPath(string path)
    {
        Directory.CreateDirectory(path);
        return path;
    }
}