using Community.VisualStudio.Toolkit;

namespace Tvl.VisualStudio.JustMyCodeToggle.Commands
{

    //do we only do the exclude list but load all others or do we automatically decide what to load
    [Command(PackageIds.JMCSymbolLoadBtn)]
    internal class FullSymbolLoadToggleCmd() : ToggleSettingCmd<int, FullSymbolLoadToggleCmd>("Debugger", "SymbolUseExcludeList", 1, 0)
    {
    }
}
