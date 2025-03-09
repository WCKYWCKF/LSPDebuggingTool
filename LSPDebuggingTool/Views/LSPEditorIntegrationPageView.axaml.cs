using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Ursa.Common;
using Ursa.Controls;
using Ursa.Controls.Options;

namespace LSPDebuggingTool.Views;

public partial class LSPEditorIntegrationPageView : UserControl
{
    public LSPEditorIntegrationPageView()
    {
        InitializeComponent();
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        var options = new DrawerOptions()
        {
            Title = "调试信息文件与位置",
            CanResize = true,
            CanLightDismiss = true,
            IsCloseButtonVisible = true,
            Buttons = DialogButton.None,
            Position = Position.Top,
            MinHeight = 500
        };
        Drawer.Show(control: new LSPServerStartInfoView(), null, options: options);
    }
}