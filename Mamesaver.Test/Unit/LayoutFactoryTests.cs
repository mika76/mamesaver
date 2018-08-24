using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using FluentAssertions;
using Mamesaver.Layout;
using NUnit.Framework;

namespace Mamesaver.Test.Unit
{
    [TestFixture]
    [TestOf(typeof(LayoutFactory))]
    public class LayoutFactoryTests
    {
        private const int MonitorWidth = 1920;
        private const int MonitorHeight = 1080;

        private const int BezelHeight = 60;

        [Test]
        public void BoundsHorizontal()
        {
            var layout = LayoutFactory.Build(MonitorWidth, MonitorHeight, BezelHeight);
            var bounds = layout.View.Screen.Bounds;

            bounds.Width.Should().Be(1360);
            bounds.Height.Should().Be(1020);
            bounds.X.Should().Be(280);
            bounds.Y.Should().Be(0);
       }

        [Test]
        public void BoundVertical()
        {
            var layout = LayoutFactory.Build(MonitorWidth, MonitorHeight, BezelHeight, false);
            var bounds = layout.View.Screen.Bounds;

            bounds.Width.Should().Be(765);
            bounds.Height.Should().Be(1020);
            bounds.X.Should().Be(578);
            bounds.Y.Should().Be(0);
        }

        [Test]
        public void BezelHorizontal()
        {
            var layout = LayoutFactory.Build(MonitorWidth, MonitorHeight, BezelHeight);
            var bounds = layout.View.Bezel.Bounds;

            bounds.Width.Should().Be(1920);
            bounds.Height.Should().Be(60);
            bounds.X.Should().Be(0);
            bounds.Y.Should().Be(1020);
        }

        [Test]
        public void BezelVertical()
        {
            var layout = LayoutFactory.Build(MonitorWidth, MonitorHeight, BezelHeight, false);
            var bounds = layout.View.Bezel.Bounds;

            bounds.Width.Should().Be(1920);
            bounds.Height.Should().Be(60);
            bounds.X.Should().Be(0);
            bounds.Y.Should().Be(1020);
        }

        [Test]
        public void SerializationHorizontal()
        {
            var layout = LayoutFactory.Build(MonitorWidth, MonitorHeight, BezelHeight);

            var stream = new MemoryStream();
            LayoutFactory.Serialize(layout, stream);
            stream.Position = 0;
            var reader = new StreamReader(stream);
            var text = reader.ReadToEnd();

            var target = Encoding.UTF8.GetString(Resources.horizontal);
            StripControlChars(text).Should().Be(StripControlChars(target));
 
        }

        [Test]
        public void SerializationVertical()
        {
            var layout = LayoutFactory.Build(MonitorWidth, MonitorHeight, BezelHeight, false);

            var stream = new MemoryStream();
            LayoutFactory.Serialize(layout, stream);
            stream.Position = 0;
            var reader = new StreamReader(stream);
            var text = reader.ReadToEnd();

            var target = Encoding.UTF8.GetString(Resources.vertical);
            StripControlChars(text).Should().Be(StripControlChars(target));
        }

        /// <summary>
        ///     Removes control characters to prevent test failures for non-structural components
        /// </summary>
        private string StripControlChars(string text) => Regex.Replace(text, @"[^\u0009\u000A\u000D\u0020-\u007E]", "");
    }
}