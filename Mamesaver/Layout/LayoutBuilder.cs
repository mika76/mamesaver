using System;
using System.IO;
using Mamesaver.Layout.Models;

namespace Mamesaver.Layout
{
    /// <summary>
    ///     Constructs and manages Mame layouts, used for rendering information about
    ///     the currently-playing game.
    /// </summary>
    public class LayoutBuilder : IDisposable
    {
        private readonly DirectoryInfo _tempDirectory;
        private bool _disposed;

        /// <summary>
        ///     Rotation for horizontal games
        /// </summary>
        private const string Horizontal = "0";

        public LayoutBuilder()
        {
            var tempPath = Path.GetTempPath();
            _tempDirectory = Directory.CreateDirectory(Path.Combine(tempPath, "Mamesaver"));
        }

        /// <summary>
        ///     Writes a Mame layout to disk. The layout contains a rendered image of the game's
        ///     metadata.
        /// </summary>
        /// <returns>art path containing temporary layout</returns>
        public string EnsureLayout(Game game, int monitorWidth, int monitorHeight)
        {
            // Identify game rotation
            var rotation = GetRotation(game);
            var horizontalGame = rotation == Horizontal;

            // Build layout and write to the temporary layout directory
            var layout = LayoutFactory.Build(monitorWidth, monitorHeight, TitleFactory.TitleHeight, horizontalGame);
            WriteLayout(game, layout);

            // Write title image
            using (var stream = new FileStream(Path.Combine(LayoutDirectory(game), LayoutConstants.TitleImage), FileMode.Create))
            {
                TitleFactory.Render(game, layout, stream, monitorWidth);
            }

            // Add our temporary art path so Mame picks up the temporary layout
            var artPath = GameListBuilder.GetArtPaths();
            artPath.Add(_tempDirectory.FullName);

           return string.Join(";", artPath);
        }

        /// <summary>
        ///     Writes Mame layout file to disk
        /// </summary>
        /// <param name="game">game to write layout for</param>
        /// <param name="layout">layout to write</param>
        private void WriteLayout(Game game, MameLayout layout)
        {
            // Ensure layout directory exists
            var layoutDirectory = LayoutDirectory(game);
            Directory.CreateDirectory(layoutDirectory);

            // Write layout
            using (var stream = new FileStream(Path.Combine(layoutDirectory, "default.lay"), FileMode.Create))
            {
                LayoutFactory.Serialize(layout, stream);
            }
        }

        private string LayoutDirectory(Game game)
        {
            return Path.Combine(_tempDirectory.FullName, game.Name);
        }

        /// <summary>
        ///     Identifies a game's rotation
        /// </summary>
        /// <param name="game"game to identify rotation for></param>
        /// <returns>rotation in degrees</returns>
        private static string GetRotation(Game game)
        {
            var rotation = game.Rotation;

            // Rotation may not be stored against the game if we're running against an old version of the screensaver without
            // the rotation field persisted, so retrieve game details again to fetch it for the current game.
            if (rotation == null)
            {
               var details = GameListBuilder.GetRomDetails(game.Name);
                rotation = details.Rotation;
            }

            // If we still can't determine the rotation, assume it's an unrotated game
            return rotation ?? Horizontal;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            try
            {
                _tempDirectory?.Delete(true);
            }
            catch (Exception e)
            {
                Program.Log(e);
            }

            _disposed = true;
        }

        ~LayoutBuilder()
        {
            Dispose(false);
        }
    }
}