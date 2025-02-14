using System;
using System.Globalization;
using System.IO;
using System.Reactive.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using LSPDebuggingTool.Models;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace LSPDebuggingTool.Models
{
    public partial class SettingStorage
    {
        public string? LastUsedAncillaryInfoFilePath
        {
            get;
            set => SetField(ref field, value);
        }
    }
}

namespace LSPDebuggingTool.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [Reactive] private string? _ancillaryInfoFilePath;

        #region CanCommand

        private IObservable<bool> _canSaveAncillaryInfo;

        #endregion

        [ObservableAsProperty] private bool _isAncillaryInfoLoaded;

        [Reactive] private LSPClientViewModel? _lSPClientViewModel;

        public MainWindowViewModel()
        {
            _lSPClientViewModel = new LSPClientViewModel();

            this.WhenAnyValue(x => x.AncillaryInfoFilePath)
                .Subscribe(x =>
                {
                    if (File.Exists(x)) SettingStorage.Instance.LastUsedAncillaryInfoFilePath = x;
                });

            _canSaveAncillaryInfo = this.WhenAnyValue(x => x.AncillaryInfoFilePath)
                .Select(x => !string.IsNullOrEmpty(x));


            _isAncillaryInfoLoadedHelper = _canSaveAncillaryInfo.ToProperty(this, nameof(IsAncillaryInfoLoaded));
        }

        public static string AppTitle { get; } = "LSP Debugging Tool (WIP)";

        [ReactiveCommand(CanExecute = nameof(_canSaveAncillaryInfo))]
        private void SaveAncillaryInfo()
        {
            RuntimeBasicInfo.Log.Information("正在保存附属信息");
            try
            {
                var str = JsonSerializer.Serialize(LSPClientViewModel,
                    LSPClientViewModelJSC.Default.LSPClientViewModel);
                File.WriteAllText(AncillaryInfoFilePath!, str);
            }
            catch (Exception e)
            {
                RuntimeBasicInfo.Log.Error(e.Message, "保存失败");
            }

            RuntimeBasicInfo.Log.Information("保存成功");
        }

        public static MainWindowViewModel Init()
        {
            RuntimeBasicInfo.Log.Information("正在初始化视图模型MainWindowViewModel");
            MainWindowViewModel mainWindowViewModel = new();
            RuntimeBasicInfo.Log.Information("检查上一次使用的附属信息文件路径是否可用");
            if (File.Exists(SettingStorage.Instance.LastUsedAncillaryInfoFilePath))
                try
                {
                    RuntimeBasicInfo.Log.Information("正在读取上一次使用的附属信息文件");
                    mainWindowViewModel.AncillaryInfoFilePath = SettingStorage.Instance.LastUsedAncillaryInfoFilePath;
                    using var fileStream = File.OpenRead(mainWindowViewModel.AncillaryInfoFilePath);
                    mainWindowViewModel.LSPClientViewModel =
                        JsonSerializer.Deserialize(fileStream, LSPClientViewModelJSC.Default.LSPClientViewModel);
                    RuntimeBasicInfo.Log.Information("读取完成");
                }
                catch (Exception e)
                {
                    RuntimeBasicInfo.Log.Error(e, "读取失败，置空上一次使用的附属信息文件路径记录。继续初始化");
                    SettingStorage.Instance.LastUsedAncillaryInfoFilePath = null;
                }

            RuntimeBasicInfo.Log.Information("初始化完成");
            return mainWindowViewModel;
        }

#pragma warning disable VSTHRD103
        [ReactiveCommand]
        private async Task CreateAncillaryInfoFileAsync(Task<string> path)
        {
            try
            {
                await path.WaitAsync(TimeSpan.FromMinutes(2));
            }
            catch (Exception e)
            {
                RuntimeBasicInfo.Log.Error(e, "不正常的等待");
            }

            if (string.IsNullOrEmpty(path.Result)) return;
            RuntimeBasicInfo.Log.Information("正在创建新的附属信息文件");
            LSPClientViewModel = new LSPClientViewModel();
            try
            {
                AncillaryInfoFilePath = path.Result;
                SettingStorage.Instance.LastUsedAncillaryInfoFilePath = path.Result;
                var str = JsonSerializer.Serialize(LSPClientViewModel,
                    LSPClientViewModelJSC.Default.LSPClientViewModel);
                await File.WriteAllTextAsync(path.Result, str);
            }
            catch (Exception e)
            {
                RuntimeBasicInfo.Log.Error(e, "实例已更新，但存入序列化或存入文件时失败。");
            }

            RuntimeBasicInfo.Log.Information("创建完成");
        }

        [ReactiveCommand]
        private async Task LoadAncillaryInfoAsync(Task<string> path)
        {
            try
            {
                await path.WaitAsync(TimeSpan.FromMinutes(2));
                if (File.Exists(path.Result))
                {
                    RuntimeBasicInfo.Log.Information("正在加载新的信息文件");
                    await using var fileStream = File.OpenRead(path.Result);
                    LSPClientViewModel =
                        JsonSerializer.Deserialize(fileStream, LSPClientViewModelJSC.Default.LSPClientViewModel);
                    SettingStorage.Instance.LastUsedAncillaryInfoFilePath = path.Result;
                    AncillaryInfoFilePath = path.Result;
                    RuntimeBasicInfo.Log.Information("加载完成");
                }
            }
            catch (Exception e)
            {
                RuntimeBasicInfo.Log.Error(e, "加载失败，实例未更新");
            }
        }
#pragma warning restore VSTHRD103

        // public ObservableCollection<TestDataItem> _testDataItems;
        // public ReadOnlyObservableCollection<TestDataItem> TestDataItems { get; set; }
        //
        // [ReactiveCommand]
        // private void AddTestDataItem(decimal year)
        // {
        //     _testDataItems.Add(new((int)year, Path.GetFileNameWithoutExtension(Path.GetRandomFileName())));
        // }
    }

    [JsonSerializable(typeof(MainWindowViewModel))]
    public partial class MainWindowViewModelJSC : JsonSerializerContext;

    public class IsExpandToGridLength : IValueConverter
    {
        public static IsExpandToGridLength Instance { get; } = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not bool isExpand) return GridLength.Auto;
            return isExpand ? GridLength.Star : GridLength.Auto;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}