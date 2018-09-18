using System.IO;
using System.Xml.Serialization;
using FluentAssertions;
using Mamesaver.Layout;
using Mamesaver.Layout.Models;
using NUnit.Framework;

namespace Mamesaver.Test.Unit
{
    [TestFixture]
    [TestOf(typeof(LayoutFactory))]
    public class LayoutFactoryTests : MamesaverTests
    {
        private LayoutFactory _layoutFactory;
        private const int MonitorWidth = 1920;
        private const int MonitorHeight = 1080;

        private const int BezelHeight = 60;
        private XmlSerializer _serializer;

        [SetUp]
        public void SetUp()
        {
            _layoutFactory = GetInstance<LayoutFactory>();
            _serializer = new XmlSerializer(typeof(MameLayout));
        }

        [Test]
        public void BoundsHorizontal()
        {
            var layout = _layoutFactory.Build(MonitorWidth, MonitorHeight, BezelHeight);
            var bounds = layout.View.Screen.Bounds;

            bounds.Width.Should().Be(1360);
            bounds.Height.Should().Be(1020);
            bounds.X.Should().Be(280);
            bounds.Y.Should().Be(0);
       }

        [Test]
        public void BoundVertical()
        {
            var layout = _layoutFactory.Build(MonitorWidth, MonitorHeight, BezelHeight, false);
            var bounds = layout.View.Screen.Bounds;

            bounds.Width.Should().Be(765);
            bounds.Height.Should().Be(1020);
            bounds.X.Should().Be(578);
            bounds.Y.Should().Be(0);
        }

        [Test]
        public void BezelHorizontal()
        {
            var layout = _layoutFactory.Build(MonitorWidth, MonitorHeight, BezelHeight);
            var bounds = layout.View.Bezel.Bounds;

            bounds.Width.Should().Be(1920);
            bounds.Height.Should().Be(60);
            bounds.X.Should().Be(0);
            bounds.Y.Should().Be(1020);
        }

        [Test]
        public void BezelVertical()
        {
            var layout = _layoutFactory.Build(MonitorWidth, MonitorHeight, BezelHeight, false);
            var bounds = layout.View.Bezel.Bounds;

            bounds.Width.Should().Be(1920);
            bounds.Height.Should().Be(60);
            bounds.X.Should().Be(0);
            bounds.Y.Should().Be(1020);
        }

        [Test]
        public void SerializationHorizontal()
        {
            var layout = _layoutFactory.Build(MonitorWidth, MonitorHeight, BezelHeight);

            var stream = new MemoryStream();
            _layoutFactory.Serialize(layout, stream);
            stream.Position = 0;
            
            // Sanity check that the expected XML deserializes to the same object
            var target = _serializer.Deserialize(new MemoryStream(Resources.horizontal));
            layout.Should().BeEquivalentTo(target);

            // Verify the layout serialization 
            var source = _serializer.Deserialize(stream);
            source.Should().BeEquivalentTo(target);
        }

        [Test]
        public void SerializationVertical()
        {
            var layout = _layoutFactory.Build(MonitorWidth, MonitorHeight, BezelHeight, false);

            var stream = new MemoryStream();
            _layoutFactory.Serialize(layout, stream);
            stream.Position = 0;
            
            // Sanity check that the expected XML deserializes to the same object
            var target = _serializer.Deserialize(new MemoryStream(Resources.vertical));
            layout.Should().BeEquivalentTo(target);

            // Verify the layout serialization 
            var source = _serializer.Deserialize(stream);
            source.Should().BeEquivalentTo(target);
        }
    }
}