using Avalonia.Controls;
using Avalonia.Controls.Templates;
using LSPDebuggingTool.ViewModels;

namespace LSPDebuggingTool.Views;

public class TitleUIViewLocator : IDataTemplate
{
    public Control? Build(object? param)
    {
        return param switch
        {
            LSPEditorIntegrationPageViewModel => new TitleUIInLSPEditorIntegrationView(),
            LSPDocumentPageViewModel => null,
            _ => null
        };
    }

    public bool Match(object? data)
    {
        return data switch
        {
            LSPEditorIntegrationPageViewModel => true,
            LSPDocumentPageViewModel => true,
            _ => false
        };
    }
}