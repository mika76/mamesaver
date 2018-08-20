using System;
using System.IO;
using System.Xml.Serialization;
using Mamesaver.Layout.Models;

namespace Mamesaver.Layout
{
    public static class LayoutFactory
    {
        /// <summary>
        ///     Serializes a layout created by <see cref="Build"/> to a stream
        /// </summary>
        public static void Serialize(MameLayout layout, Stream output)
        {
            var serializer = new XmlSerializer(typeof(MameLayout));
            serializer.Serialize(output, layout);
        }

        /// <summary>
        ///     Creates a new layout
        /// </summary>
        /// <param name="monitorWidth">horizontal resolution of monitor</param>
        /// <param name="monitorHeight">vertical resolution of monitor</param>
        /// <param name="bezelHeight">height of bezel image. It is assumed that the bezel's width is the horizontal resolution of the monitor</param>
        /// <param name="horizontal">if the game is in the horizontal rotation</param>
        /// <returns>layout</returns>
        public static MameLayout Build(int monitorWidth, int monitorHeight, int bezelHeight, bool horizontal = true)
        {
            // Scale game so bezel doesn't overlap game
            var scale = (monitorHeight - bezelHeight) / (float) monitorHeight;

            // Mame games are either 3:4 or 4:3 aspect ratio depending on whether they're horizontal or vertical
            var screenRatio = 4 / (float)3;
            if (!horizontal) screenRatio = 1 / screenRatio;

            // Apply scale of Mame game
            var screenWidth = (int)Math.Round(screenRatio * monitorHeight * scale);
            var screenHeight = monitorHeight * scale;

            return new MameLayout
            {
                Version = LayoutConstants.Version,
                Element = new Element { Name = "bezel", Image = new Image { File = LayoutConstants.TitleImage } },
                View = new View
                {
                    Name = "Mamesaver",
                    Screen = new Screen
                    {
                        Index = 0,
                        Bounds = new Bounds
                        {
                            X = (int)Math.Round(monitorWidth / 2.0 - screenWidth / 2.0),
                            Y = 0,
                            Width = screenWidth,
                            Height = (int)Math.Round(screenHeight)
                        }
                    },
                    Bezel = new Bezel
                    {
                        Element = "bezel",
                        Bounds = new Bounds
                        {
                            X = 0,
                            Y = monitorHeight - bezelHeight,
                            Width = monitorWidth,
                            Height = bezelHeight 
                        }
                    }
                }
            };
        }
    }
}