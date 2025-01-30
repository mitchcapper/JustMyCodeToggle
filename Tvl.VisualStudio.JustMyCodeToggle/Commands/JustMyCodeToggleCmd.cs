using System.Threading.Tasks;
using Community.VisualStudio.Toolkit;
using EnvDTE;


namespace Tvl.VisualStudio.JustMyCodeToggle.Commands
{
    [Command(PackageIds.JMCMenuController)]
    internal class MenuBtnCmd : BaseCommand<MenuBtnCmd>
    {
        public MenuBtnCmd()
        {
            instance = this;
        }
        internal static MenuBtnCmd instance;

    }

    [Command(PackageIds.JMCJustMyCodeBtn)]
    internal class JustMyCodeToggleCmd() : ToggleSettingCmd<int, JustMyCodeToggleCmd>("Debugger", "JustMyCode", 1, 0)
    {
        protected override void SetCheckedToMatch(int cur)
        {
            base.SetCheckedToMatch(cur);
            if (MenuBtnCmd.instance != null)
                MenuBtnCmd.instance.Command.Checked = Command.Checked; //keep menu in sync with us
        }
        protected override Task InitializeCompletedAsync()
        {
            DelaySetChecked();
            return base.InitializeCompletedAsync();
        }
        /// <summary>
        /// to set the related command items not initalized before
        /// </summary>
        private async void DelaySetChecked()
        {
            await Task.Delay(1000);
            //await JoinableTaskFactory.SwitchToMainThreadAsync();
            SetCheckedToMatch(Command.Checked ? 1 : 0);

        }
    }
}
