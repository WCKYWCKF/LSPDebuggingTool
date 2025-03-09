using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace LSPDebuggingTool.Views;

public partial class MainPageView : UserControl
{
    public static readonly StyledProperty<Control> NowPageProperty = AvaloniaProperty.Register<MainPageView, Control>(
        nameof(NowPage));

    public Control NowPage
    {
        get => GetValue(NowPageProperty);
        private set => SetValue(NowPageProperty, value);
    }

    public MainPageView()
    {
        InitializeComponent();
    }
}