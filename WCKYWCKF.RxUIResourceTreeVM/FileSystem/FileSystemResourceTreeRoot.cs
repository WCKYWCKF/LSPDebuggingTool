using System.Collections.ObjectModel;
using System.Reactive.Linq;
using ReactiveMarbles.ObservableEvents;

namespace WCKYWCKF.RxUIResourceTreeVM.FileSystem;

public class FileSystemResourceTreeRoot : ResourceItemRootBase
{
    protected readonly ObservableCollection<ResourceItemBase> _items;
    protected readonly FileSystemWatcher _watcher;

    public FileSystemResourceTreeRoot()
    {
        _items = new ObservableCollection<ResourceItemBase>();
        Items = new ReadOnlyObservableCollection<ResourceItemBase>(_items);
        _watcher = new FileSystemWatcher();
        _watcher.NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName;
        _watcher.InternalBufferSize = 64 * 1024;
        _watcher.IncludeSubdirectories = true;

        OnFileSystemItemCreated = _watcher.Events().Created;
        OnFileSystemItemRenamed = _watcher.Events().Renamed;
        OnFileSystemItemDeleted = _watcher.Events().Deleted;
        OnFileSystemItemChanged = _watcher.Events().Changed;

        OnFileSystemItemCreated.Do(OnFileSystemItemCreatedHandler).Subscribe();
        OnFileSystemItemRenamed.Do(OnFileSystemItemRenamedHandler).Subscribe();
        OnFileSystemItemDeleted.Do(OnFileSystemItemDeletedHandler).Subscribe();
    }

    public IObservable<FileSystemEventArgs> OnFileSystemItemCreated { get; }
    public IObservable<RenamedEventArgs> OnFileSystemItemRenamed { get; }
    public IObservable<FileSystemEventArgs> OnFileSystemItemDeleted { get; }

    public IObservable<FileSystemEventArgs> OnFileSystemItemChanged { get; }

    public string? RootPath { get; private set; }

    public override ReadOnlyObservableCollection<ResourceItemBase> Items { get; }

    private void OnFileSystemItemCreatedHandler(FileSystemEventArgs obj)
    {
        if (GetByItemPath(Path.GetDirectoryName(obj.FullPath)!) is FileSystemResourceItem item)
            item._children.Add(CreateFileSystemResourceItem(obj.FullPath));
    }

    private void OnFileSystemItemRenamedHandler(RenamedEventArgs obj)
    {
        if (GetByItemPath(obj.OldFullPath) is FileSystemResourceItem item) item.Header = obj.Name;
    }

    private void OnFileSystemItemDeletedHandler(FileSystemEventArgs obj)
    {
        if (GetByItemPath(obj.FullPath) is FileSystemResourceItem item) item.Parent?._children.Remove(item);
    }

    protected ResourceItemBase? GetByItemPath(string path)
    {
        var headers = path.Remove(0, RootPath!.Length).Split(Path.DirectorySeparatorChar);
        var result = Items.FirstOrDefault(x => x.Header == headers[0]);
        if (result == null) return result;
        foreach (var header in headers.Skip(1))
        {
            if (result == null) return null;
            result = result.Children.FirstOrDefault(x => x.Header == header);
        }

        return result;
    }

    public void Start(string path)
    {
        if (Directory.Exists(path) is false)
            throw new DirectoryNotFoundException(path);
        _watcher.Path = path;
        _watcher.EnableRaisingEvents = true;
        RootPath = Path.GetDirectoryName(_watcher.Path)!;
        _items.Add(CreateFileSystemResourceItem(path));
    }


    public virtual FileSystemResourceItem CreateFileSystemResourceItem(string path)
    {
        return new FileSystemResourceItem(path, this);
    }

    public void Stop()
    {
        _watcher.EnableRaisingEvents = false;
        _items.Clear();
    }

    ~FileSystemResourceTreeRoot()
    {
        _watcher.EnableRaisingEvents = false;
        _watcher.Dispose();
    }
}