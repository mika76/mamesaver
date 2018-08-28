namespace Mamesaver.Configuration
{
    /// <inheritdoc />
    /// <summary>
    ///     Settings store for general screensaver configuration.
    /// </summary>
    public class GeneralSettingsStore : SettingsStore<Models.Settings>
    {
        public override string Filename => "settings.xml";
    }
}
