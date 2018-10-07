using Mamesaver.Models.Configuration;
using Mamesaver.Power;

namespace Mamesaver
{
    /// <summary>
    ///     Convenience factory to create blank screens.
    /// </summary>
    internal class BlankScreenFactory
    {
        private readonly LayoutSettings _layoutSettings;
        private readonly PowerManager _powerManager;

        public BlankScreenFactory(LayoutSettings layoutSettings, PowerManager powerManager)
        {
            _layoutSettings = layoutSettings;
            _powerManager = powerManager;
        }

        /// <summary>
        ///     Creates a new <see cref="BlankScreen"/>.
        /// </summary>
        public BlankScreen Create() => new BlankScreen(_layoutSettings, _powerManager);
    }
}