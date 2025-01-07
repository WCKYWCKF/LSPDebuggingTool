using System;
using System.IO;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace LSPDebuggingTool.ViewModels;

public abstract partial class TVEItemBase : ViewModelBase
{
    protected readonly CompositeDisposable _disposables = new();
    [Reactive] private FileSystemInfo _info;
    [ObservableAsProperty] private string _name;
    [ObservableAsProperty] private string _parentPath;
    [ObservableAsProperty] private string _path;

    protected TVEItemBase(FileSystemInfo info, Func<FileSystemInfo, string> getParentPath)
    {
        _name = _parentPath = _path = null!;
        Info = info;
        var share = this.WhenAnyValue(x => x.Info);
        _nameHelper = share.Select(x => x.Name).ToProperty(this, nameof(Name));
        _parentPathHelper = share.Select(getParentPath).ToProperty(this, nameof(ParentPath));
        _pathHelper = share.Select(x => x.FullName).ToProperty(this, nameof(Path));
    }
}