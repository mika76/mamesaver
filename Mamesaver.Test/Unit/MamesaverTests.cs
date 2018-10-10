using Mamesaver.Services.Configuration;
using NUnit.Framework;
using SimpleInjector;

namespace Mamesaver.Test.Unit
{
    public abstract class MamesaverTests
    {
        public Container Container { get; private set; }

        [OneTimeSetUp]
        public void SetupContainer() => Container = ContainerFactory.NewContainer();

        /// <summary>
        ///     Retrieves an instance from the container
        /// </summary>
        public T GetInstance<T>() where T : class => Container.GetInstance<T>();
    }

    /// <summary>
    ///     Copy from main application due to ILMerge's internalisation resulting in the <see cref="NewContainer"/>
    ///     method being made internal.
    /// </summary>
    public static class ContainerFactory
    {
        public static Container NewContainer()
        {
            var container = new Container();

            container.Register(() => container.GetInstance<GeneralSettingsStore>().Get());
            container.Register(() => container.GetInstance<GameListStore>().GetGameList);

            return container;
        }
    }
}