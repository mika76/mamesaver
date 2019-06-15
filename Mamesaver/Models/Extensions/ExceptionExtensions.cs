using System.IO;

namespace Mamesaver.Models.Extensions
{
    public static class ExceptionExtensions
    {
        /// <summary>
        ///     Extracts the path from a <see cref="DirectoryNotFoundException"/>
        /// </summary>
        public static string GetPath(this DirectoryNotFoundException e)
        {
            var pathMatcher = new System.Text.RegularExpressions.Regex(@"[^']+");
            return pathMatcher.Matches(e.Message)[1].Value;
        }
    }
}
