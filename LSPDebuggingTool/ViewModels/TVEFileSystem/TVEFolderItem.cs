using System;
using System.Collections.ObjectModel;
using System.IO;
using DynamicData;
using DynamicData.Binding;

namespace LSPDebuggingTool.ViewModels;

public class TVEFolderItem : TVEItemBase
{
    internal readonly Lazy<ReadOnlyObservableCollection<TVEItemBase>> Children;
    internal readonly ObservableCollection<FileSystemInfo> Files = [];

    public TVEFolderItem(DirectoryInfo info) : base(info, x => (x as DirectoryInfo)?.Parent?.FullName ?? string.Empty)
    {
        Children = new Lazy<ReadOnlyObservableCollection<TVEItemBase>>(() =>
        {
            Files.AddRange(info.GetFileSystemInfos());
            Files.ToObservableChangeSet(x => x.FullName)
                .Transform(x =>
                {
                    TVEItemBase? item = x switch
                    {
                        DirectoryInfo dir => new TVEFolderItem(dir),
                        FileInfo file => new TVEFileItem(file),
                        _ => null
                    };

                    return item ?? throw new Exception();
                })
                .SortAndBind(out var items, SortExpressionComparer<TVEItemBase>.Descending(x => x is TVEFolderItem)
                    .ThenByAscending(x => x.Name))
                .Subscribe();
            return items;
        });
    }

    public ReadOnlyObservableCollection<TVEItemBase> Items => Children.Value;
}