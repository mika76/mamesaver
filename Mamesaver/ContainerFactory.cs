using System;
using Mamesaver.Configuration;
using Mamesaver.Configuration.Models;
using SimpleInjector;

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

            // Register setting factories
            container.Register(() => container.GetInstance<GeneralSettingsStore>().Get(), Lifestyle.Singleton);
            container.Register(() => container.GetInstance<GameListStore>().GetGameList, Lifestyle.Singleton);
            container.Register(() => container.GetInstance<Settings>().LayoutSettings, Lifestyle.Singleton);
            container.Register(() => container.GetInstance<Settings>().AdvancedSettings, Lifestyle.Singleton);

            // Register screen factory
            container.Register(() => new Func<BlankScreen>(() => container.GetInstance<BlankScreen>()));

            return container;
        }
    }
}