using System.Threading.Tasks;

namespace LSPDebuggingTool;

public static class WCKY_UIToolsetExtensions
{
    public static async Task<string?> TryGetStringAsync(this object source)
    {
        return source switch
        {
            string str => str,
            Task<string> strAsync => await strAsync,
            _ => null
        };
    }
}