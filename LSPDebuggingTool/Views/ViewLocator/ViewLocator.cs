using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace LSPDebuggingTool.Views;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? param)
    {
        throw new System.NotImplementedException();
    }

    public bool Match(object? data)
    {
        return false;
    }
}