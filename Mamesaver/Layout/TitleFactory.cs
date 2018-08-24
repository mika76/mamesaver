using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;
using Mamesaver.Layout.Models;
using Mamesaver.Models;
using Mamesaver.Models.Settings;

namespace Mamesaver.Layout
{
    /// <summary>
    ///     Generates an image containing game metadata for display in a Mame layout.
    /// </summary>
    public static class TitleFactory
    {
        private static readonly Color BackgroundColour = Color.Black;
        private static readonly Color TextColour = Color.FromArgb(255, 210, 210, 210);

    private class TitleFonts
    {
        public Font RegularFont { get; set; }
        public Font BoldFont { get; set; }
        }

        private static TitleFonts GetTitleFonts()
        {
            var fontSettings = FontSettings();

            return new TitleFonts
            {
                RegularFont = new Font(fontSettings.Face, fontSettings.Size, FontStyle.Regular, GraphicsUnit.Point),
                BoldFont = new Font(fontSettings.Face, fontSettings.Size, FontStyle.Bold, GraphicsUnit.Point)
            };
        }

        public static int GetBezelHeight(Game game) => GetBezelHeight(GetTitleSize(game));

        private static int GetBezelHeight(Size titleSize) => (int)Math.Round(titleSize.Height * 1.3f);

        private static Size GetTitleSize(Game game)
        {
            var boldFont = GetTitleFonts().BoldFont;
            return TextRenderer.MeasureText(game.Description, boldFont);
        }

        /// <summary>
        ///     Render a layout image for a game. The image's width is determined by <see cref="monitorWidth"/> and height
        ///     relative to the font height.
        /// </summary>
        public static int Render(Game game, MameLayout layout, Stream outputStream, int monitorWidth)
        {
            var bezelHeight = GetBezelHeight(game);
            var titleSize = GetTitleSize(game);
            var fonts = GetTitleFonts();

            using (var image = new Bitmap(monitorWidth, bezelHeight, PixelFormat.Format32bppArgb))
            using (var graphics = Graphics.FromImage(image))
            {
                PrepareGraphics(graphics);

                // Vertically centre text
                var yOffset = (int)Math.Round(bezelHeight / 2.0 - titleSize.Height / 2.0);
                var xOffset = layout.View.Screen.Bounds.X;

                // Render game description, manufacturer and year
                TextRenderer.DrawText(graphics, game.Description, fonts.BoldFont, new Point(xOffset, yOffset), TextColour);
                TextRenderer.DrawText(graphics, $"| {game.Manufacturer} {game.Year}", fonts.RegularFont, new Point(xOffset + titleSize.Width, yOffset), TextColour);

                graphics.Save();
                image.Save(outputStream, ImageFormat.Png);
            }

            return bezelHeight;
        }

        private static FontSettings FontSettings()
        {
            var settings = SettingStores.General.Get();
            var layoutSettings = settings.LayoutSettings.InGameTitles;
            var fontSettings = layoutSettings.FontSettings;
            return fontSettings;
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