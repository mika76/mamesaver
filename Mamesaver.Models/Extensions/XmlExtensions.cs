using System.Xml.Linq;

namespace Mamesaver.Models.Extensions
{
    public static class XmlExtensions
    {
        /// <summary>
        ///     Converts a MAME yes/no attribute to a boolean, or <c>null</c> if the attribute isn't found.
        /// </summary>
        public static bool? ToBoolean(this XElement element, string attributeName)
        {
            var attribute = element.Attribute(attributeName)?.Value;
            if (attribute == null) return null;

            return attribute == "yes";
        }
    }
}