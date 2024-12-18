using Avalonia.Controls;
using LSPDebuggingTool.ViewModels;
using Ursa.ReactiveUIExtension;

namespace LSPDebuggingTool.Views;

public partial class MainWindow : ReactiveUrsaWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
    }
}