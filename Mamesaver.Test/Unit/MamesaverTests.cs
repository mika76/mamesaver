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
}