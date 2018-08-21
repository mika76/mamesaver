using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;
using Mamesaver.Layout.Models;

namespace Mamesaver.Layout
{
    /// <summary>
    ///     Generates an image containing game metadata for display in a Mame layout.
    /// </summary>
    public static class TitleFactory
    {
        /// <summary>
        ///     Font size in pixels
        /// </summary>
        private const int FontSize = 18;

        private const string FontFace = "Arial";

        public static int TitleHeight { get; } = 30;

        private static readonly Font RegularFont = new Font(FontFace, FontSize, FontStyle.Regular, GraphicsUnit.Pixel);
        private static readonly Font BoldFont = new Font(FontFace, FontSize, FontStyle.Bold, GraphicsUnit.Pixel);

        private static readonly Color BackgroundColour = Color.Black;
        private static readonly Color TextColour = Color.FromArgb(255, 210, 210, 210);

        /// <summary>
        ///     Render a layout image for a game. The image's width is determined by <see cref="monitorWidth"/> and height
        ///     by <see cref="TitleHeight"/>.
        /// </summary>
        public static void Render(Game game, MameLayout layout, Stream outputStream, int monitorWidth)
        {
            using (var image = new Bitmap(monitorWidth, TitleHeight, PixelFormat.Format32bppArgb)) 
            using (var graphics = Graphics.FromImage(image))
            {
                PrepareGraphics(graphics);

                // Render game description, manufacturer and year
                var titleSize = TextRenderer.MeasureText(game.Description, BoldFont);

                // Vertically centre text
                var yOffset = (int)Math.Round(TitleHeight / 2.0 - titleSize.Height / 2.0);
                var xOffset = layout.View.Screen.Bounds.X;

                TextRenderer.DrawText(graphics, game.Description, BoldFont, new Point(xOffset, yOffset), TextColour);
                TextRenderer.DrawText(graphics, $"| {game.Manufacturer} {game.Year}", RegularFont, new Point(xOffset + titleSize.Width, yOffset), TextColour);

                graphics.Save();
                image.Save(outputStream, ImageFormat.Png);
            }
        }

        /// <summary>
        ///     Adjusts a <see cref="Graphics"/> object for high quality rendering
        /// </summary>
        /// <param name="graphics"></param>
        private static void PrepareGraphics(Graphics graphics)
        {
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            graphics.Clear(BackgroundColour);
        }
    }
}