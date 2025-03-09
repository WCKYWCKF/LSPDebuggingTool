using System.Text.Json.Serialization;

namespace LSPDebuggingTool.ViewModels;

[JsonSourceGenerationOptions(WriteIndented = true, IgnoreReadOnlyFields = true, IgnoreReadOnlyProperties = true)]
[JsonSerializable(typeof(LSPClientViewModel))]
public partial class LSPClientViewModelJSC : JsonSerializerContext;