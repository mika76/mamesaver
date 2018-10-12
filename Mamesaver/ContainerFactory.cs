using Mamesaver.Config.Filters.ViewModels;
using Mamesaver.Config.ViewModels;
using Mamesaver.Hotkeys;
using Mamesaver.Layout;
using Mamesaver.Models.Configuration;
using Mamesaver.Power;
using Mamesaver.Services;
using Mamesaver.Services.Categories;
using Mamesaver.Services.Configuration;
using Mamesaver.Services.Mame;
using Mamesaver.Services.Windows;
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
            container.Register<MameScreen>(Lifestyle.Scoped);
            container.Register<GamePlayManager>(Lifestyle.Scoped);
            container.Register<CaptureScreen>(Lifestyle.Scoped);
            container.Register<ScreenCloner>(Lifestyle.Scoped);
            container.Register<ScreenManager>(Lifestyle.Scoped);
            container.Register<PowerManager>(Lifestyle.Scoped);
            container.Register<HotKeyManager>(Lifestyle.Scoped);
            container.Register<MameOrchestrator>(Lifestyle.Scoped);
            container.Register<BlankScreenFactory>(Lifestyle.Scoped);

            container.Register<GameListBuilder>(Lifestyle.Singleton);
            container.Register<TitleFactory>(Lifestyle.Singleton);
            container.Register<LayoutBuilder>(Lifestyle.Singleton);
            container.Register<LayoutFactory>(Lifestyle.Singleton);
            container.Register<MameInvoker>(Lifestyle.Singleton);
            container.Register<PowerEventWatcher>(Lifestyle.Singleton);
            container.Register<MamePathManager>(Lifestyle.Singleton);
            container.Register<ServiceResolver>(Lifestyle.Singleton);
            container.Register<CategoryParser>(Lifestyle.Singleton);

            container.Register<Config.ConfigForm>(Lifestyle.Singleton);
            container.Register<ConfigFormViewModel>(Lifestyle.Singleton);
            container.Register<DecadeFilterViewModel>(Lifestyle.Singleton);

            container.Register<IActivityHook>(() => new UserActivityHook(typeof(Program).Assembly), Lifestyle.Singleton);

            return container;
        }
    }
}