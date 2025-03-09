using WCKYWCKF.RxUIResourceTreeVM.FileSystem;

namespace LSPDebuggingTool.ViewModels;

public class LSPEditorIntegrationPageViewModel : ViewModelBase
{
    public FileSystemResourceTreeRoot Root { get; }

    public LSPEditorIntegrationPageViewModel()
    {
        Root = new FileSystemResourceTreeRoot();
        Root.Start(@"D:\Temp");
    }
}