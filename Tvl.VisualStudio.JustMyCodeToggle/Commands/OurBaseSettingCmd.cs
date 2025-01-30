using System.Threading.Tasks;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using Tvl.VisualStudio.JustMyCodeToggle.Managers;


namespace Tvl.VisualStudio.JustMyCodeToggle.Commands
{
    internal abstract class ToggleSettingCmd<SETTING_TYPE, OUR_CLASS>(string collectionPath, string propertyName, SETTING_TYPE enabled_val, SETTING_TYPE disabled_val) : OurSettingCmd<SETTING_TYPE, OUR_CLASS>(collectionPath, propertyName) where OUR_CLASS : class, new()
    {
        
        protected override Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            var cur = _settingsManager.GetSetting<SETTING_TYPE>(collectionPath, propertyName);
            cur = cur.Equals(disabled_val) ? enabled_val : disabled_val;
            SetValue(cur);
            SetCheckedToMatch(cur);


            return base.ExecuteAsync(e);
        }
        protected virtual void SetCheckedToMatch(SETTING_TYPE cur)
        {
            if (!cur.Equals(disabled_val))
                Command.Checked = true;
            else
                Command.Checked = false;
        }
        protected override async Task InitializeCompletedAsync()
        {
            await base.InitializeCompletedAsync();
            SetCheckedToMatch(GetValue());
            
        }
    }
    internal abstract class OurSettingCmd<SETTING_TYPE, OUR_CLASS> : BaseCommand<OUR_CLASS> where OUR_CLASS : class, new()
    {
        protected readonly string collectionPath;
        protected readonly string propertyName;
        protected SettingsManager _settingsManager;

        public OurSettingCmd(string collectionPath, string propertyName)
        {

            this.collectionPath = collectionPath;
            this.propertyName = propertyName;
        }
    protected override async Task InitializeCompletedAsync()
    {
            await base.InitializeCompletedAsync();
            _settingsManager = Package.GetTypedService<SettingsManager>();
      
    }

    protected SETTING_TYPE GetValue() => _settingsManager.GetSetting<SETTING_TYPE>(collectionPath, propertyName);
        protected void SetValue(SETTING_TYPE val) => _settingsManager.SetSetting(collectionPath, propertyName, val);

    }
}
