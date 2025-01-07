using System;
using System.Globalization;
using System.IO;
using Avalonia.Data.Converters;

namespace LSPDebuggingTool.Views;

public sealed class AncillaryInfoFilePathConverter : IValueConverter
{
    public static AncillaryInfoFilePathConverter Default { get; } = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string path) return string.Empty;
        if (path.Length > 20)
            path = string.Concat(path.AsSpan()[..10], $"...{Path.DirectorySeparatorChar}", Path.GetFileName(path));
        return path;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}