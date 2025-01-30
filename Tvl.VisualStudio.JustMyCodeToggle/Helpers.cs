using System.Threading.Tasks;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Tvl.VisualStudio.JustMyCodeToggle
{

    /// <summary>
    /// Extension methods for Visual Studio project types
    /// </summary>
    public static class ProjectExtensions
    {
        public static T GetTypedService<T>(this AsyncPackage package) where T : class => package.GetService<T, T>();
        public static async Task<EnvDTE.Project> GetDTEProjectFromIvsProject(this IVsProject proj)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            ((IVsHierarchy)proj).GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ExtObject, out object prop);
            return prop as EnvDTE.Project;
        }

        public static async Task<string> GetCurrentConfigurationName(this IVsProject project)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var hProject = project as IVsHierarchy;
            hProject.GetGuidProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ProjectIDGuid, out var projGuid);
            var bm = await VS.Services.GetSolutionBuildManagerAsync() as IVsSolutionBuildManager5;
            bm.FindActiveProjectCfgName(ref projGuid, out var currentConfigName);
            return currentConfigName;
        }

        public static async Task SaveProject(this IVsProject project, bool force)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var solution = await VS.Services.GetSolutionAsync();
            solution.SaveSolutionElement(
                force ? (uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_ForceSave : (uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_SaveIfDirty,
                project as IVsHierarchy,
                0);
        }
    }


}
