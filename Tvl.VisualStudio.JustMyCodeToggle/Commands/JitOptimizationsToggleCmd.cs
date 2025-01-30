using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using Tvl.VisualStudio.JustMyCodeToggle.Managers;

namespace Tvl.VisualStudio.JustMyCodeToggle.Commands
{
    [Command(PackageIds.JMCDisableJitOptmizationsBtn)]
    internal class JitOptimizationsToggleCmd : BaseCommand<JitOptimizationsToggleCmd>
    {

        private StartupProjectManager _startupProjectManager;
        private LaunchProfileManager _launchProfileManager;

        public JitOptimizationsToggleCmd()
        {
            var baseVars = new Dictionary<string, string>(){ {"ZapDisable","1" },
                        {"ReadyToRun","0" },
                        {"TieredCompilation","0" },
                        {"JITMinOpts","1" }};
            var envs = new List<KeyValuePair<string, string>>();
            string[] prefixes = ["DOTNET_", "COMPlus_"];
            foreach (var kvp in baseVars)
            {
                foreach (var prefix in prefixes)
                    envs.Add(new(prefix + kvp.Key, kvp.Value));
            }
            ENVVars = envs.ToArray();
        }
        private KeyValuePair<string, string>[] ENVVars;
        protected override async Task InitializeCompletedAsync()
        {
            await base.InitializeCompletedAsync();
                        _startupProjectManager = Package.GetTypedService<StartupProjectManager>();
            _launchProfileManager = Package.GetTypedService<LaunchProfileManager>();

            await UpdateCheckedState();
            _startupProjectManager.StartupProjectChanged += (_, _) => UpdateCheckedState();
            
        }

        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            await SetValue(!Command.Checked);
            await UpdateCheckedState();
        }

        private async Task SetValue(bool value)
        {
            if (!_startupProjectManager.ActiveProjectSupportsCPSProfiles)
                return;//shouldn't get here
            await _launchProfileManager.UpdateLaunchProfileENVVars(isDelete: !value, ENVVars.ToArray());

        }

        private async Task UpdateCheckedState()
        {
            Command.Enabled = _startupProjectManager.HasStartupProject && _startupProjectManager.ActiveProjectSupportsCPSProfiles;
            if (!Command.Enabled)
                return;
            var launchProfile = await _launchProfileManager.GetLaunchProfile();
            if (launchProfile == null)
            {
                Command.Enabled = false;
                return;
            }

            var envVars = launchProfile.EnvironmentVariables;
            var zapDisable = launchProfile?.EnvironmentVariables.FirstOrDefault(a => a.Key == "COMPlus_ZapDisable");

            Command.Checked = zapDisable?.Value == "1";


        }
    }
}
