using System.IO;
using Mamesaver.Layout;
using Mamesaver.Layout.Models;
using NUnit.Framework;

namespace Mamesaver.Test.Unit
{
 
    [TestFixture]
    public class TitleFactoryTests : MamesaverTests
    {
        private TitleFactory _titleFactory;

        [SetUp]
        public void SetUp() => _titleFactory = GetInstance<TitleFactory>();

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

                var layout = new MameLayout { View = new View { Screen = new Screen { Bounds = new Bounds { X = 1000 } } } };
                _titleFactory.Render(game, layout, stream, 1920);
            }
        }
    }
}