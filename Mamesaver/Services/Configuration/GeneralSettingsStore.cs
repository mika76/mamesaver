using Mamesaver.Models.Configuration;

namespace Mamesaver.Services.Configuration
{
    /// <inheritdoc />
    /// <summary>
    ///     Settings store for general screensaver configuration.
    /// </summary>
    public class GeneralSettingsStore : SettingsStore<Settings>
    {
        public override string Filename => "settings.xml";
    }
}
