using Mamesaver.Services.Properties;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Mamesaver.Services.Categories
{
    public class CategoryParser
    {
        private static readonly Regex SectionRegex = new Regex(@"\s*\[(.*)\]\s*");
        private static readonly Regex DetailRegex = new Regex(@"([^=]+)=(.+)");

        private const string CategorySection = "Category";

        private static readonly Dictionary<string, string> Categories = new Dictionary<string, string>();

        public string GetCategory(string game)
        {
            if (!Categories.Any())
            {
                Load();
            }

            return Categories.ContainsKey(game) ? Categories[game] : "";
        }

        private static void Load()
        {

            var categorySection = false;
            using (var reader = new StringReader(Resource.catver))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    // Stop parsing if we have read all games from the category section
                    if (categorySection && IsSectionLine(line)) return;

                    // Scan for category section
                    if (!categorySection && IsCategorySection(line))
                    {
                        categorySection = true;
                        continue;
                    }

                    if (IsGameLine(line))
                    {
                        RegisterGame(line);
                    }
                }
            }
        }

        private static void RegisterGame(string line)
        {
            var match = DetailRegex.Match(line);
            Categories[match.Groups[1].Value] = match.Groups[2].Value;
        }

        private static bool IsGameLine(string line) => DetailRegex.IsMatch(line);
        private static bool IsSectionLine(string line) => SectionRegex.IsMatch(line);

        private static bool IsCategorySection(string line)
        {
            if (!IsSectionLine(line)) return false;
            return SectionRegex.Match(line).Groups[1].Value == CategorySection;
        }
    }
}