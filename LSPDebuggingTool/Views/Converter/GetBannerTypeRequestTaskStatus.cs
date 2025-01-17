using System;
using System.Globalization;
using Avalonia.Controls.Notifications;
using Avalonia.Data.Converters;
using LSPDebuggingTool.ViewModels;

namespace LSPDebuggingTool.Views;

public sealed class GetBannerTypeRequestTaskStatus : IValueConverter
{
    public static GetBannerTypeRequestTaskStatus Instance { get; } = new GetBannerTypeRequestTaskStatus();
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            RequestTaskStatus.Pending or RequestTaskStatus.Running => NotificationType.Information,
            RequestTaskStatus.Failed => NotificationType.Error,
            RequestTaskStatus.Completed => NotificationType.Success,
            _ => NotificationType.Warning
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
} 