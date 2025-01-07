using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using DynamicData;

namespace LSPDebuggingTool.ViewModels;

public class ObservableCollectionFileSystems : IDisposable
{
    private readonly TVEFolderItem _folderItem;
    private readonly FileSystemWatcher _watcher;

    public ObservableCollectionFileSystems(string rootPath)
    {
        _watcher = new FileSystemWatcher(rootPath);
        _watcher.NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName;
        _watcher.InternalBufferSize = 64 * 1024;
        _watcher.IncludeSubdirectories = true;
        _watcher.EnableRaisingEvents = true;
        _watcher.Created += (sender, args) =>
        {
            var parentDirPath = TryGetFolder(args.FullPath) ??
                                throw new InvalidOperationException("Could not find folder");
            parentDirPath.Files.Add(Directory.Exists(args.FullPath)
                ? new DirectoryInfo(args.FullPath)
                : new FileInfo(args.FullPath));
        };
        _watcher.Deleted += (sender, args) =>
        {
            var parentDirPath = TryGetFolder(args.FullPath) ??
                                throw new InvalidOperationException("Could not find folder");
            parentDirPath.Files.Remove(parentDirPath.Files.FirstOrDefault(x => x.FullName == args.FullPath) ??
                                       throw new InvalidOperationException("Could not find file or folder"));
        };
        _watcher.Renamed += (sender, args) =>
        {
            var parentDirPath = TryGetFolder(args.FullPath) ??
                                throw new InvalidOperationException("Could not find folder");
            var item = parentDirPath.Files.FirstOrDefault(x => x.FullName == args.OldFullPath) ??
                       throw new InvalidOperationException("Could not find file or folder");
            parentDirPath.Files.Replace(item, Directory.Exists(args.FullPath)
                ? new DirectoryInfo(args.FullPath)
                : new FileInfo(args.FullPath));
        };

        DirectoryInfo root = new(rootPath);
        _folderItem = new TVEFolderItem(root);
    }

    public ReadOnlyObservableCollection<TVEItemBase> Items => _folderItem.Items;

    public void Dispose()
    {
        _watcher.Dispose();
    }

    private TVEFolderItem? TryGetFolder(string fullPath)
    {
        fullPath = fullPath.Replace(_folderItem.Path, string.Empty);
        string[] path = fullPath.Split(Path.DirectorySeparatorChar).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        var items = _folderItem;
        for (var i = 0; i < path.Length - 1; i++)
        {
            items = items.Items.OfType<TVEFolderItem>().FirstOrDefault(x => x.Name == path[i]);
            if (items is null)
                return null;
        }

        return items;
    }
}