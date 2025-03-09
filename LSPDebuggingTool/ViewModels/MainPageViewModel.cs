using ReactiveUI;

namespace LSPDebuggingTool.ViewModels;

public class MainPageViewModel : ViewModelBase, IScreen
{
    public RoutingState Router { get; } = new ();
}