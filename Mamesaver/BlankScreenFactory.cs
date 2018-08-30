using Mamesaver.Configuration.Models;

namespace Mamesaver
{
    /// <summary>
    ///     Convenience factory to create blank screens.
    /// </summary>
    internal class BlankScreenFactory
    {
        private readonly LayoutSettings _layoutSettings;
        private readonly Settings _settings;

        public BlankScreenFactory(LayoutSettings layoutSettings, Settings settings)
        {
            _layoutSettings = layoutSettings;
            _settings = settings;
        }

        /// <summary>
        ///     Creates a new <see cref="BlankScreen"/>.
        /// </summary>
        public BlankScreen Create() => new BlankScreen(new BackgroundForm(_layoutSettings), _settings);
    }
}