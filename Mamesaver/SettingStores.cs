using Mamesaver.Settings;

namespace Mamesaver
{
    public static class SettingStores
    {
        /// <summary>
        ///     Game list and selection state
        /// </summary>
        public static GameListStore GameList { get; } 

        /// <summary>
        ///     User settings
        /// </summary>
        public static GeneralSettingsStore General { get; }

        static SettingStores()
        {
            GameList = new GameListStore();
            General = new GeneralSettingsStore();
        }
    }
}
