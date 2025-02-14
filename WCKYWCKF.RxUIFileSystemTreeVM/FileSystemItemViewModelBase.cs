using ReactiveUI;

namespace WCKYWCKF.RxUIFileSystemTreeVM;

public class FileSystemItemViewModelBase : ReactiveObject
{
    protected FileSystemItemViewModelBase(string path)
    {
        Path = path;
        Name = System.IO.Path.GetFileName(path);
        ParentName = System.IO.Path.GetDirectoryName(path) ?? string.Empty;
    }

    public string Name
    {
        get;
        internal set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public string Path
    {
        get;
        internal set
        {
            if (EnableNotificationsForPath)
                this.RaiseAndSetIfChanged(ref field, value);
            else field = value;
        }
    }

    public string ParentName
    {
        get;
        internal set
        {
            if (EnableNotificationsForParentPath)
                this.RaiseAndSetIfChanged(ref field, value);
            else field = value;
        }
    }

    public bool EnableNotificationsForPath { get; set; }
    public bool EnableNotificationsForParentPath { get; set; }
}