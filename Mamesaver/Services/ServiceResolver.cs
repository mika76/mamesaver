using System;
using SimpleInjector;

namespace Mamesaver.Services
{
    /// <summary>
    ///     Provides static resolution of Simple Injector instances.
    /// </summary>
    public class ServiceResolver
    {
        private Container Container { get; }
        private static ServiceResolver Resolver { get; set; }

        public ServiceResolver(Container container) => Container = container;

        /// <summary>
        ///     Initialises the static resolver.
        /// </summary>
        public void Initialise() => Resolver = this;

        public static T GetInstance<T>()
        {
            if (Resolver == null) throw new InvalidOperationException($"{nameof(ServiceResolver)} must be constructed prior to use.");
            return (T) Resolver.Container.GetInstance(typeof(T));
        }
    }
}
