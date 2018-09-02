using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;
using Mamesaver.Configuration.Models;
using Mamesaver.Layout.Models;
using Serilog;

namespace Mamesaver.Layout
{
    /// <summary>
    ///     Generates an image containing game metadata for display in a MAME layout.
    /// </summary>
    public class TitleFactory
    {
        private readonly Color _backgroundColour = Color.Black;
        private readonly Settings _settings;
        private readonly Color _textColour = Color.FromArgb(255, 210, 210, 210);

        public TitleFactory(Settings settings) => _settings = settings;

        /// <summary>
        ///     Returns the height of the bezel containing the rendered title
        /// </summary>
        public int GetBezelHeight(Game game) => GetBezelHeight(GetDescriptionSize(game));

        /// <summary>
        ///     Returns the height of the bezel containing the rendered title.
        /// </summary>
        /// <param name="titleSize">size of title text</param>
        private int GetBezelHeight(Size titleSize) => (int)Math.Round(titleSize.Height * 1.3f);

        /// <summary>
        ///     Returns the size of the rendered game description text.
        /// </summary>
        private Size GetDescriptionSize(Game game)
        {
            var font = GetTitleFonts().BoldFont;
            return TextRenderer.MeasureText(DescriptionText(game), font);
        }

        /// <summary>
        ///     Returns the size of the rendered game details text.
        /// </summary>
        private Size GetDetailSize(Game game)
        {
            var font = GetTitleFonts().RegularFont;
            return TextRenderer.MeasureText(DetailText(game), font);
        }

        /// <summary>
        ///     Render a layout image for a game. The image's width is determined by <see cref="monitorWidth" /> and height
        ///     relative to the font height.
        /// </summary>
        public int Render(Game game, MameLayout layout, Stream outputStream, int monitorWidth)
        {
            Log.Information("Rendering game titles");

            var bezelHeight = GetBezelHeight(game);
            var titleSize = GetDescriptionSize(game);
            var fonts = GetTitleFonts();

            using (var image = new Bitmap(monitorWidth, bezelHeight, PixelFormat.Format32bppArgb))
            using (var graphics = Graphics.FromImage(image))
            {
                PrepareGraphics(graphics);

                // Vertically centre text inside bezel
                var yOffset = (int) Math.Round(bezelHeight / 2.0 - titleSize.Height / 2.0);

                // Horizontally centre game on screen
                var textWidth = GetDetailSize(game).Width + GetDescriptionSize(game).Width;
                var xOffset = (int) Math.Round(monitorWidth / 2.0 - textWidth / 2.0);

                // Render game description, manufacturer and year
                TextRenderer.DrawText(graphics, DescriptionText(game), fonts.BoldFont, new Point(xOffset, yOffset),
                    _textColour);
                TextRenderer.DrawText(graphics, DetailText(game), fonts.RegularFont,
                    new Point(xOffset + titleSize.Width, yOffset), _textColour);

                graphics.Save();
                image.Save(outputStream, ImageFormat.Png);
            }

            return bezelHeight;
        }

        private string DescriptionText(Game game) => game.Description;

        private string DetailText(Game game) => $"| {game.Manufacturer} {game.Year}";

        private TitleFonts GetTitleFonts()
        {
            var fontSettings = FontSettings();

            return new TitleFonts
            {
                RegularFont = new Font(fontSettings.Face, fontSettings.Size, FontStyle.Regular, GraphicsUnit.Point),
                BoldFont = new Font(fontSettings.Face, fontSettings.Size, FontStyle.Bold, GraphicsUnit.Point)
            };
        }

        private FontSettings FontSettings()
        {
            var layoutSettings = _settings.LayoutSettings.InGameTitles;
            return layoutSettings.FontSettings;
        }

        /// <summary>
        ///     Adjusts a <see cref="Graphics" /> object for high quality rendering.
        /// </summary>
        /// <param name="graphics"></param>
        private void PrepareGraphics(Graphics graphics)
        {
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            graphics.Clear(_backgroundColour);
        }

        private class TitleFonts
        {
            public Font RegularFont { get; set; }
            public Font BoldFont { get; set; }
        }
    }
}