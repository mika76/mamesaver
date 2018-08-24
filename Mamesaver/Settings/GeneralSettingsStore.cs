using Mamesaver.Models.Settings;

namespace Mamesaver.Settings
{
    public class GeneralSettingsStore : SettingsStore<GeneralSettings>
    {
        public override string Filename => "settings.xml";
    }
}
