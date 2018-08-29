using System;
using Mamesaver.Configuration.Models;

namespace Mamesaver
{
    /// <summary>
    ///     Convenience factory to create blank screens.
    /// </summary>
    internal class BlankScreenFactory
    {
        private readonly LayoutSettings _layoutSettings;

        public BlankScreenFactory(LayoutSettings layoutSettings) => _layoutSettings = layoutSettings;

        /// <summary>
        ///     Creates a new <see cref="BlankScreen"/>.
        /// </summary>
        public BlankScreen Create()
        {
            return new BlankScreen(new BackgroundForm(_layoutSettings));
        }
    }
}