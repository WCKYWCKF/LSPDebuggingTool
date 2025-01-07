using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    private async Task OpenTextFileAsync(TVEFileItem item)
    {
        _openedTexts.Add(item);
        await item.OpemCommand.Execute(Unit.Default);
    }

    [ReactiveCommand]
    private async Task RemoveTextFileAsync(TVEFileItem item)
    {
        return;
        _openedTexts.Remove(item);
        await item.CloseCommand.Execute(Unit.Default);
    }
}