using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace LSPDebuggingTool.Models;

public sealed class LogReader : ObservableCollection<string>, IDisposable
{
    private readonly CompositeDisposable _disposable = new();

    private FileStream? fileStream;
    private StreamReader? streamReader;
    private IDisposable? updateSubscription;

    public LogReader()
    {
        Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                x => PropertyChanged += x,
                x => PropertyChanged -= x)
            .Where(x => x.EventArgs.PropertyName == nameof(Path))
            .Do(_ =>
            {
                Clear();
                fileStream?.Dispose();
                streamReader?.Dispose();
                updateSubscription?.Dispose();
                updateSubscription = null;
                fileStream = null;
                streamReader = null;
                if (!File.Exists(Path)) return;
                fileStream = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                streamReader = new StreamReader(fileStream);
                UpdateItems();
                updateSubscription = Observable.Interval(TimeSpan.FromSeconds(1))
                    .Do(_ => UpdateItems())
                    .Subscribe();
            })
            .Subscribe()
            .DisposeWith(_disposable);
    }

    public string? Path
    {
        get;
        set
        {
            if (value == field) return;
            field = value;
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Path)));
        }
    }

    public void Dispose()
    {
        fileStream?.Dispose();
        streamReader?.Dispose();
    }

    private void UpdateItems()
    {
        if (streamReader is null) return;

        var str = streamReader.ReadLine();
        while (string.IsNullOrEmpty(str) is false)
        {
            Add(str);
            str = streamReader.ReadLine();
        }
    }
}