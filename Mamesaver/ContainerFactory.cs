using Mamesaver.Configuration;
using Mamesaver.Configuration.Models;
using Mamesaver.Layout;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace Mamesaver
{
    /// <summary>
    ///     Initialises the Simple Injector DI container.
    /// </summary>
    public static class ContainerFactory
    {
        /// <summary>
        ///     Creates a new Simple Injector container
        /// </summary>
        /// <returns></returns>
        public static Container NewContainer()
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            // Register setting stores
            container.Register<GameListStore>(Lifestyle.Singleton);
            container.Register<GeneralSettingsStore>(Lifestyle.Singleton);

            // Register setting factories
            container.Register(() => container.GetInstance<GeneralSettingsStore>().Get(), Lifestyle.Singleton);
            container.Register(() => container.GetInstance<GameListStore>().GetGameList, Lifestyle.Singleton);
            container.Register(() => container.GetInstance<Settings>().LayoutSettings, Lifestyle.Singleton);
            container.Register(() => container.GetInstance<Settings>().AdvancedSettings, Lifestyle.Singleton);

            // Register components requiring explicit lifestyles
            container.Register<BackgroundForm>(Lifestyle.Scoped);
            container.Register<MameScreen>(Lifestyle.Scoped);

            container.Register<GameListBuilder>(Lifestyle.Singleton);
            container.Register<TitleFactory>(Lifestyle.Singleton);
            container.Register<LayoutBuilder>(Lifestyle.Singleton);
            container.Register<LayoutFactory>(Lifestyle.Singleton);
            container.Register<MameInvoker>(Lifestyle.Singleton);

            return container;
        }
    }
}