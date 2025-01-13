using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace LSPDebuggingTool.ViewModels;

public partial class LSPClientViewModel
{
    private readonly ObservableCollection<TVEFileItem> _openedTexts = [];
    [ObservableAsProperty] private ObservableCollectionFileSystems? _fileSystems;

    [JsonIgnore] public ReadOnlyObservableCollection<TVEFileItem> OpenedTexts { get; }

    [JsonIgnore]
    public TVEFileItem? SelectedOpenedText
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    [ReactiveCommand]
    private void OpenTextFile(TVEFileItem item)
    {
        if (_openedTexts.Any(x => x.Path == item.Path)) return;
        _openedTexts.Add(item);
    }
}