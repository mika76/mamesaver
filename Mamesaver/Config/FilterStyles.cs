using System.Windows;
using Mamesaver.Properties;

namespace Mamesaver.Config
{
    public static class FilterStyles
    {
        /// <summary>
        ///     Filter icon template.
        /// </summary>
        [NotNull]
        public static readonly ResourceKey IconTemplateKey = new ComponentResourceKey(typeof(FilterStyles), "IconTemplate");

        /// <summary>
        ///     Filter icon style.
        /// </summary>
        [NotNull]
        public static readonly ResourceKey IconStyleKey = new ComponentResourceKey(typeof(FilterStyles), "IconStyle");
    }
}