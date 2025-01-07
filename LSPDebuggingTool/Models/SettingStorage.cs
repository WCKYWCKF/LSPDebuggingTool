using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LSPDebuggingTool.Models;

public sealed partial class SettingStorage : INotifyPropertyChanged
{
    static SettingStorage()
    {
        RuntimeBasicInfo.Log.Information("开始加载设置");
        Load();
        Instance.PropertyChanged += (sender, args) => Save();
    }

    public static SettingStorage Instance { get; private set; } = null!;


    public event PropertyChangedEventHandler? PropertyChanged;

    private static void Load()
    {
        Instance = new SettingStorage();
        if (File.Exists(RuntimeBasicInfo.SettingPath))
        {
            using var fileStream = File.OpenRead(RuntimeBasicInfo.SettingPath);
            try
            {
                var result = JsonSerializer.Deserialize(fileStream, SettingStorageJSC.Default.SettingStorage);
                if (result is null)
                    RuntimeBasicInfo.Log.Error("在反序列化设置文件中的内容时返回null。忽略设置文件并继续执行，所有设置均为默认。运行期间当设置值发生更改时将尝试存储设置信息");
                else
                    Instance = result;
            }
            catch (Exception e)
            {
                RuntimeBasicInfo.Log.Error(e, "在反序列化设置文件中的内容时出错。忽略设置文件并继续执行，所有设置均为默认。运行期间当设置值发生更改时将尝试存储设置信息");
            }
        }
        else
        {
            RuntimeBasicInfo.Log.Error("存储软件设置信息的文件缺失，开始重建");
            try
            {
                File.WriteAllText(
                    RuntimeBasicInfo.SettingPath,
                    JsonSerializer.Serialize(Instance, SettingStorageJSC.Default.SettingStorage));
                RuntimeBasicInfo.Log.Information("设置文件重建完成，所有设置恢复默认");
            }
            catch (Exception e)
            {
                RuntimeBasicInfo.Log.Error(e, "设置文件重建失败。忽略设置文件并继续执行，所有设置均为默认。运行期间当设置值发生更改时将尝试存储设置信息");
            }
        }

        RuntimeBasicInfo.Log.Information("设置加载完成");
    }

    private static void Save()
    {
        RuntimeBasicInfo.Log.Information("开始保存设置");
        try
        {
            var json = JsonSerializer.Serialize(Instance, SettingStorageJSC.Default.SettingStorage);
            File.WriteAllText(RuntimeBasicInfo.SettingPath, json);
            RuntimeBasicInfo.Log.Information("设置保存成功");
        }
        catch (Exception e)
        {
            RuntimeBasicInfo.Log.Information(e, "设置保存失败");
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

[JsonSerializable(typeof(SettingStorage))]
public partial class SettingStorageJSC : JsonSerializerContext;