using System;
using System.IO;
using System.Reactive.Linq;
using Akavache;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace LSPDebuggingTool.Models;

public partial class SettingsStorage : ReactiveObject
{
    [ObservableAsProperty] private string? _ancillaryInformationPath;

    [ObservableAsProperty] private string? _logPath;

    private SettingsStorage()
    {
        _logPathHelper = this.WhenAnyValue(x => x.BasicPath)
            .Select(x => Directory.Exists(x) ? CheckPath(Path.Combine(x, "logs")) : null)
            .ToProperty(this, nameof(LogPath));
        _ancillaryInformationPathHelper = this.WhenAnyValue(x => x.BasicPath)
            .Select(x => Directory.Exists(x) ? CheckPath(Path.Combine(x, "ancillaryInformation")) : null)
            .ToProperty(this, nameof(LogPath));
    }

    public static SettingsStorage? Instance { get; private set; }

    public bool IsFirstEntryToPortraitOrientation
    {
        get => BlobCache.UserAccount.GetOrCreateObject(nameof(IsFirstEntryToPortraitOrientation), () =>
            false).GetAwaiter().GetResult();
        set => BlobCache.UserAccount.InsertObject(nameof(IsFirstEntryToPortraitOrientation), value);
    }

    public string? BasicPath
    {
        get => BlobCache.UserAccount.GetOrCreateObject<string>(nameof(BasicPath), () =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                nameof(LSPDebuggingTool))).GetAwaiter().GetResult();
        set
        {
            if (Directory.Exists(value) is false)
                throw new DirectoryNotFoundException(value);
            BlobCache.UserAccount.InsertObject(nameof(BasicPath), value);
            this.RaiseAndSetIfChanged(ref field, value);
        }
    }

    private string CheckPath(string path)
    {
        Directory.CreateDirectory(path);
        return path;
    }

    public static void Initialize()
    {
        Instance ??= new SettingsStorage();
    }
}