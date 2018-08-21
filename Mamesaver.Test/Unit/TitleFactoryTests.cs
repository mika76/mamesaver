using System.IO;
using Mamesaver.Layout;
using NUnit.Framework;

namespace Mamesaver.Test.Unit
{
 
    [TestFixture]
    public class TitleFactoryTests
    {
        [Test]
        [Description("Sanity check that the title rendering doesn't blow up")]
        public void RenderTitle()
        {
            using (var stream = new MemoryStream())
            {
                var game = new Game
                {
                    Description = "Pin the tail on the turnip",
                    Manufacturer = "Bongo vision",
                    Year = "1953"
                };

                TitleFactory.Render(game, stream, 1920);
            }
        }
    }
}