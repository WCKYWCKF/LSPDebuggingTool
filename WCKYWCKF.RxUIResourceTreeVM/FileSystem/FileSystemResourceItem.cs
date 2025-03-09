using System.Collections.ObjectModel;
using DynamicData;
using DynamicData.Binding;

namespace WCKYWCKF.RxUIResourceTreeVM.FileSystem;

public class FileSystemResourceItem : ResourceItemBase
{
    public FileSystemResourceItem(string path, ResourceItemRootBase root) : base(root)
    {
        if (File.Exists(path))
        {
            Header = Path.GetFileName(path);
            IsFolder = false;
        }
        else
        {
            Header = new DirectoryInfo(path).Name;
            IsFolder = true;
            _children.Add(Empty);
        }
    }

    public bool IsFolder { get; }

    internal override ObservableCollection<ResourceItemBase> _children { get; } = [];

    protected override ReadOnlyObservableCollection<ResourceItemBase> CreateChildren()
    {
        var sort = SortExpressionComparer<ResourceItemBase>.Descending(x => x is FileSystemResourceItem)
            .ThenByDescending(x => (x as FileSystemResourceItem)?.IsFolder ?? false)
            .ThenByAscending(x => x.Header!);
        _children.ToObservableChangeSet(x => x.Id)
            .Filter(x => x.Header is not "$RECYCLE.BIN")
            .SortAndBind(out var list, sort)
            .Subscribe();
        return list;
    }

    protected override void LoadChildren()
    {
        if (Root is FileSystemResourceTreeRoot root)
        {
            DirectoryInfo directoryInfo = new(GetPath());
            _children.AddRange(
                directoryInfo.GetDirectories().Select(x => root.CreateFileSystemResourceItem(x.FullName)));
            _children.AddRange(
                directoryInfo.GetFiles().Select(x => root.CreateFileSystemResourceItem(x.FullName)));
        }
    }

    public string GetPath()
    {
        if (Root is FileSystemResourceTreeRoot root)
        {
            var path = string.Empty;
            ResourceItemBase? parent = this;
            do
            {
                path = Path.Combine(parent.Header!, path);
                parent = parent.Parent;
            } while (parent != null);


            return Path.Combine(root.RootPath!, path);
        }

        throw new ArrayTypeMismatchException(nameof(Root));
    }
}