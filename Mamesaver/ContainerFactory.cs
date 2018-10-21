using Mamesaver.Config.Filters;
using Mamesaver.Config.ViewModels;
using Mamesaver.Config.ViewModels.AdvancedTab;
using Mamesaver.Config.ViewModels.GameListTab;
using Mamesaver.Config.ViewModels.GeneralTab;
using Mamesaver.Config.ViewModels.HotkeysTab;
using Mamesaver.Config.ViewModels.LayoutTab;
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
            container.Register<CategoryParser>(Lifestyle.Singleton);

            container.Register<Config.ConfigForm>(Lifestyle.Singleton);
            container.Register<ConfigViewModel>(Lifestyle.Singleton);
            container.Register<GameListViewModel>(Lifestyle.Singleton);
            container.Register<LayoutViewModel>(Lifestyle.Singleton);
            container.Register<GeneralViewModel>(Lifestyle.Singleton);
            container.Register<AdvancedViewModel>(Lifestyle.Singleton);
            container.Register<HotKeysViewModel>(Lifestyle.Singleton);

            container.Register<MultipleChoiceFilterViewModel>(Lifestyle.Transient);

            //container.Register<IActivityHook>(() => new UserActivityHook(typeof(Program).Assembly), Lifestyle.Singleton);
            container.Register<IActivityHook>(() => new UserActivityHook(), Lifestyle.Singleton);
            container.Register(() => new ServiceResolver(container), Lifestyle.Singleton);

            return container;
        }
    }
}