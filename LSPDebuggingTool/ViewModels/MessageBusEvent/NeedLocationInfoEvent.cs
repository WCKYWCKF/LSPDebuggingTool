using System;

namespace LSPDebuggingTool.ViewModels.MessageBusEvent;

public sealed class NeedLocationInfoEvent
{
    public LocationInfo? LocationInfo { get; set; }
}