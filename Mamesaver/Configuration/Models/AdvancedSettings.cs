using System.Xml.Serialization;

namespace Mamesaver.Configuration.Models
{
    [XmlRoot("Advanced")]
    public class AdvancedSettings
    {
        /// <summary>
        ///     Include games which have imperfect emulation.
        /// </summary>
        /// <remarks>
        ///     This is useful if running a custom MAME build with the mandatory imperfect emulation
        ///     screen disabled.
        /// </remarks>
        [XmlElement("IncludeImperfectEmulation")]
        public bool IncludeImperfectEmulation { get; set; }

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