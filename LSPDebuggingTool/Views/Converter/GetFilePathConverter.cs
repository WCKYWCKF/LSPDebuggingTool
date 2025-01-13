using System;
using System.Globalization;
using Avalonia.Data.Converters;
using LSPDebuggingTool.ViewModels;

namespace LSPDebuggingTool.Views;

public class GetFilePathConverter : IValueConverter
{
    public static GetFilePathConverter Instance { get; } = new GetFilePathConverter();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (value as TVEFileItem)?.Path;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}