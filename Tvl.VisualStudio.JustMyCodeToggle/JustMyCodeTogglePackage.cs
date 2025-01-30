// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Tvl.VisualStudio.JustMyCodeToggle
{
    using System;

    using System.Runtime.InteropServices;
    using System.Threading;
    using Community.VisualStudio.Toolkit;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Tvl.VisualStudio.JustMyCodeToggle.Managers;
    using Task = System.Threading.Tasks.Task;

    [Guid(PackageGuids.guidJustMyCodeTogglePackageCmdSetString)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideMenuResource(1000, 1)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExistsAndFullyLoaded_string, PackageAutoLoadFlags.BackgroundLoad)]
    internal class JustMyCodeTogglePackage : ToolkitPackage
    {

        protected ProjectEventHandler ProjectEventHandler;

        private StartupProjectManager _startupProjectManager;
        private LaunchProfileManager _launchProfileManager;
        private SettingsManager _settingsManager;

        internal T RegisterService<T>(T service){
            AddService(typeof(T),(_,_,_) => Task.FromResult<object>(service),promote:true);
            return service;
        }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {

            //await Helpers.Init();
            // Initialize managers
            _startupProjectManager = RegisterService(new StartupProjectManager());
            _launchProfileManager = RegisterService(new LaunchProfileManager(_startupProjectManager));
            _settingsManager = RegisterService(new SettingsManager((IVsSettingsManager)await VS.Services.GetSettingsManagerAsync()));

            await this.RegisterCommandsAsync();
            await base.InitializeAsync(cancellationToken, progress);
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            var globalSolutionSvc = await VS.GetServiceAsync<SVsSolution, IVsSolution>();
            ProjectEventHandler = new(_startupProjectManager);
            globalSolutionSvc.AdviseSolutionEvents(ProjectEventHandler, out _);
            var monitor = await VS.GetServiceAsync<SVsShellMonitorSelection, IVsMonitorSelection>();
            monitor.AdviseSelectionEvents(ProjectEventHandler, out _);
            var bm = await VS.Services.GetSolutionBuildManagerAsync();
            bm.AdviseUpdateSolutionEvents(ProjectEventHandler, out _);

            AfterLoad();
        }


        private async void AfterLoad()
        {
            await Task.Delay(2000);
            _startupProjectManager.CheckStartupProjectChanged(true);
        }

    }
}
