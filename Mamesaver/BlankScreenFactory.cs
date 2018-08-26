using System;

namespace Mamesaver
{
    /// <summary>
    ///     Convenience factory to create blank screens.
    /// </summary>
    internal class BlankScreenFactory
    {
        private readonly Func<BlankScreen> _factory;

        public BlankScreenFactory(Func<BlankScreen> factory) => _factory = factory;

        /// <summary>
        ///     Creates a new <see cref="BlankScreen"/>.
        /// </summary>
        public BlankScreen Create() => _factory();
    }
}