using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace LSPDebuggingTool.Views;

public partial class PathPicker : UserControl
{
    public enum UsePickerTypes
    {
        OpenFile,
        SaveFile,
        OpenFolder
    }

    public static readonly StyledProperty<string?> SelectedPathProperty =
        AvaloniaProperty.Register<PathPicker, string?>(
            nameof(SelectedPath), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<string> SuggestedStartPathProperty =
        AvaloniaProperty.Register<PathPicker, string>(
            nameof(SuggestStartPath), string.Empty);

    public static readonly StyledProperty<UsePickerTypes> UsePickerTypeProperty =
        AvaloniaProperty.Register<PathPicker, UsePickerTypes>(
            nameof(UsePickerType));

    public static readonly StyledProperty<string> SuggestedFileNameProperty =
        AvaloniaProperty.Register<PathPicker, string>(
            nameof(SuggestedFileName), string.Empty);

    public static readonly StyledProperty<string> FileFilterProperty = AvaloniaProperty.Register<PathPicker, string>(
        nameof(FileFilter), string.Empty);

    public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<PathPicker, string>(
        nameof(Title), string.Empty);

    public static readonly StyledProperty<string> DefaultFileExtensionProperty =
        AvaloniaProperty.Register<PathPicker, string>(
            nameof(DefaultFileExtension), string.Empty);

    public PathPicker()
    {
        InitializeComponent();
    }

    public string SuggestedFileName
    {
        get => GetValue(SuggestedFileNameProperty);
        set => SetValue(SuggestedFileNameProperty, value);
    }

    public string DefaultFileExtension
    {
        get => GetValue(DefaultFileExtensionProperty);
        set => SetValue(DefaultFileExtensionProperty, value);
    }


    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string FileFilter
    {
        get => GetValue(FileFilterProperty);
        set => SetValue(FileFilterProperty, value);
    }

    public UsePickerTypes UsePickerType
    {
        get => GetValue(UsePickerTypeProperty);
        set => SetValue(UsePickerTypeProperty, value);
    }

    public string SuggestStartPath
    {
        get => GetValue(SuggestedStartPathProperty);
        set => SetValue(SuggestedStartPathProperty, value);
    }

    public string? SelectedPath
    {
        get => GetValue(SelectedPathProperty);
        set => SetValue(SelectedPathProperty, value);
    }

#pragma warning disable VSTHRD100
    private async void LaunchPicker(object? sender, RoutedEventArgs e)
    {
        if (TopLevel.GetTopLevel(this)?.StorageProvider is not { } storageProvider) return;

        switch (UsePickerType)
        {
            case UsePickerTypes.OpenFile:
                FilePickerOpenOptions filePickerOpenOptions = new()
                {
                    Title = Title,
                    AllowMultiple = false,
                    SuggestedStartLocation = await storageProvider.TryGetFolderFromPathAsync(SuggestStartPath),
                    FileTypeFilter = FileFilter.Split(",").Select(x => new FilePickerFileType(x)).ToList()
                };
                var resFiles = await storageProvider.OpenFilePickerAsync(filePickerOpenOptions);
                if (resFiles.Count != 0)
                    SelectedPath = resFiles[0].TryGetLocalPath();
                break;
            case UsePickerTypes.SaveFile:
                FilePickerSaveOptions filePickerSaveOptions = new()
                {
                    Title = Title,
                    SuggestedStartLocation = await storageProvider.TryGetFolderFromPathAsync(SuggestStartPath),
                    SuggestedFileName = SuggestedFileName,
                    FileTypeChoices = FileFilter.Split(",").Select(x => new FilePickerFileType(x)).ToList(),
                    DefaultExtension = DefaultFileExtension
                };
                SelectedPath = (await storageProvider.SaveFilePickerAsync(filePickerSaveOptions))?.TryGetLocalPath();
                break;
            case UsePickerTypes.OpenFolder:
                FolderPickerOpenOptions folderPickerOpenOptions = new()
                {
                    Title = Title,
                    AllowMultiple = false,
                    SuggestedStartLocation = await storageProvider.TryGetFolderFromPathAsync(SuggestStartPath),
                    SuggestedFileName = SuggestedFileName
                };
                var resFolder = await storageProvider.OpenFolderPickerAsync(folderPickerOpenOptions);
                if (resFolder.Count != 0)
                    SelectedPath = resFolder[0].TryGetLocalPath();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
#pragma warning restore VSTHRD100
}