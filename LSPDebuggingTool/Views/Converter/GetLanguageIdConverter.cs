using System;
using System.Globalization;
using Avalonia.Data.Converters;
using LSPDebuggingTool.ViewModels;

namespace LSPDebuggingTool.Views;

public class GetLanguageIdConverter : IValueConverter
{
    public static GetLanguageIdConverter Instance { get; } = new GetLanguageIdConverter();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (value as LanguageIdentifier)?.LanguageId;
    }
}