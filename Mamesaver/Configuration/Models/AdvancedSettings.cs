using System.Xml.Serialization;

namespace Mamesaver.Configuration.Models
{
    [XmlRoot("Advanced")]
    public class AdvancedSettings
    {
        /// <summary>
        ///     Disable status checks on MAME ROMs.
        /// </summary>
        /// <remarks>
        ///     This is usfeul if running a custom MAME build with the mandatory imperfect emulation
        ///     screen disabled.
        /// </remarks>
        [XmlElement("SkipGameValidation")]
        public bool SkipGameValidation { get; set; }

        /// <summary>
        ///     Enable debug logging to disk.
        /// </summary>
        /// <remarks>
        ///     Debug logging is enabled in debug builds; this setting also enables it in release builds
        /// </remarks>
        [XmlElement("DebugLogging")]
        public bool DebugLogging { get; set; }
    }
}