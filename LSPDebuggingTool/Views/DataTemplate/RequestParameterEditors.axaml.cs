using Avalonia.Controls;
using Avalonia.Interactivity;

namespace LSPDebuggingTool.Views;

public partial class RequestParameterEditors : UserControl
{
    public RequestParameterEditors()
    {
        InitializeComponent();
    }

    private void HideFlyoutInListBox(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox { Tag: Flyout flyout })
        {
            flyout.Hide();
        }
    }
}