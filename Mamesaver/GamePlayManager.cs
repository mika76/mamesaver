using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Mamesaver.Configuration;
using Mamesaver.Configuration.Models;
using Mamesaver.Extensions;
using Mamesaver.Hotkeys;
using Mamesaver.Layout;
using Mamesaver.Power;
using Serilog;

namespace Mamesaver
{
    /// <summary>
    ///     Manages starting and stopping MAME and responding to hotkey events.
    /// </summary>
    internal class GamePlayManager : IDisposable
    {
        private readonly Settings _settings;
        private readonly LayoutBuilder _layoutBuilder;
        private readonly GameList _gameList;
        private readonly GameListStore _gameListStore;
        private readonly HotKeyManager _hotKeyManager;
        private readonly PowerManager _powerManager;
        private readonly MameInvoker _invoker;

        public delegate void PlayGameEventHandler(object sender, EventArgs args);
        public delegate void StartGameEventHandler(object sender, EventArgs args);
        public delegate void GameStartedEventHandler(object sender, EventArgs args);

        /// <summary>
        ///     Game has been started by MAME.
        /// </summary>
        public event GameStartedEventHandler OnGameStarted;

        /// <summary>
        ///    Game is about to be run by MAME.
        /// </summary>
        public event StartGameEventHandler OnStartGame;

        /// <summary>
        ///     User has chosen to play the current game.
        /// </summary>
        public event PlayGameEventHandler OnPlayGame;

        /// <summary>
        ///     MAME process running the current game, or <c>null</c> if not started.
        /// </summary>
        private Process _mameProcess;

        /// <summary>
        ///     Shuffled games that will be played.
        /// </summary>
        private List<Game> _selectedGames;

        /// <summary>
        ///     Index of game in <see cref="_selectedGames"/> which is being played.
        /// </summary>
        private int _gameIndex;

        private CancellationTokenSource _cancellationTokenSource;
        private bool _initialised;
        private Screen _screen;

        public GamePlayManager(
            Settings settings,
            LayoutBuilder layoutBuilder,
            GameList gameList,
            GameListStore gameListStore,
            HotKeyManager hotKeyManager,
            PowerManager powerManager,
            MameInvoker invoker)
        {
            _settings = settings;
            _layoutBuilder = layoutBuilder;
            _gameList = gameList;
            _gameListStore = gameListStore;
            _hotKeyManager = hotKeyManager;
            _powerManager = powerManager;
            _invoker = invoker;
        }

        public void Initialise(Screen screen, CancellationTokenSource cancellationTokenSource)
        {
            _screen = screen;
            _cancellationTokenSource = cancellationTokenSource;

            _powerManager.SleepTriggered += OnSleep;
            _selectedGames = _gameList.SelectedGames.Shuffle();
            _hotKeyManager.HotKeyPressed += ProcessHotKey;

            _initialised = true;
        }

        /// <summary>
        ///     Game which is currently being played.
        /// </summary>
        public Game CurrentGame() => _selectedGames[_gameIndex];

        /// <summary>
        ///     Handles sleep events, closing MAME.
        /// </summary>
        private void OnSleep(object sender, EventArgs args)
        {
            Log.Information("Stopping MAME due to sleep event");
            _powerManager.SleepTriggered -= OnSleep;
            _invoker.Stop(_mameProcess);
        }

        /// <summary>
        ///     Executes MAME, running the currently selected game.
        /// </summary>
        public void StartGame()
        {
            // End the currently playing game
            if (!_mameProcess.HasExited) _invoker.Stop(_mameProcess);
            OnStartGame?.Invoke(this, null);
        }

        /// <summary>
        ///     Selects the next game for play.
        /// </summary>
        public void NextGame()
        {
            _gameIndex++;
            if (_gameIndex >= _selectedGames.Count) _gameIndex = 0;
        }

        /// <summary>
        ///     Selects the previous game for play.
        /// </summary>
        private void PreviousGame()
        {
            _gameIndex--;
            if (_gameIndex < 0) _gameIndex = _selectedGames.Count - 1;
        }

        /// <summary>
        ///     Deselects a game, saving configuration and playing the next game
        /// </summary>
        private void DeselectGame(Game game)
        {
            // Sanity check that the user isn't attempting to deselect their last selected game
            if (_selectedGames.Count == 1)
            {
                Log.Information("Attempting to deselect single remaining game; ignoring request");
                return;
            }

            Log.Information("Deselecting {game}", game.Name);

            // Update game selection and update store
            _gameListStore.ChangeSelection(game.Name, false);
            _selectedGames.Remove(game);
            if (_gameIndex >= _selectedGames.Count) _gameIndex = 0;

            // Start next game
            StartGame();
        }

        /// <summary>
        ///     Hands control over to MAME to allow the user to play a game.
        /// </summary>
        private void PlayGame(Game game)
        {
            // Unsubscribe from hotkey events as control is being handed over to MAME
            _hotKeyManager.HotKeyPressed -= ProcessHotKey;

            OnPlayGame?.Invoke(this, null);

            // Close existing MAME instance running in screensaver
            _invoker.Stop(_mameProcess);

            // Run MAME without screensaver options
            _invoker.Run(game.Name).WaitForExit(int.MaxValue);

            // Close screensaver after game has terminated
            _cancellationTokenSource.Cancel();
        }

        /// <summary>
        ///     Handle a hot key event.
        /// </summary>
        private void ProcessHotKey(object sender, HotKeyEventArgs e)
        {
            var currentGame = CurrentGame();

            switch (e.HotKey)
            {
                case HotKey.NextGame:
                    Log.Information("Skipping to next game");
                    NextGame();
                    StartGame();
                    break;

                case HotKey.PreviousGame:
                    Log.Information("Skipping to previous game");
                    PreviousGame();
                    StartGame();
                    break;

                case HotKey.DeselectGame:
                    DeselectGame(currentGame);
                    break;

                case HotKey.PlayGame:
                    PlayGame(currentGame);
                    break;
            }
        }

        /// <summary>
        ///     Event handler for MAME exiting. If the exit code indicates a failure, the game is
        ///     deselected. This can occur if the ROM is no longer available or is corrupt.
        /// </summary>
        private void OnMameExited(object sender, EventArgs e)
        {
            _mameProcess.Exited -= OnMameExited;

            var process = (Process)sender;
            if (process.ExitCode == 0) return;

            // If MAME exited with an error, deselect the current game
            if (process.ExitCode.In(MameErrorCodes.RequireFilesMissing, MameErrorCodes.UnknownSystem))
            {
                var game = CurrentGame();
                Log.Warning("MAME exited unexpectedly playing {game}", game.Name);

                DeselectGame(CurrentGame());
            }
        }

        /// <summary>
        ///     Runs the current MAME game.
        /// </summary>
        public void RunGame()
        {
            // Don't attempt to start MAME process if we are exiting
            if (_cancellationTokenSource.IsCancellationRequested) return;

            var game = CurrentGame();

            Log.Information("Running game {description} {year} {manufacturer} on display {display}",
                game.Description,
                game.Year, game.Manufacturer, _screen.DeviceName);

            // Create layout and run game
            var arguments = new List<string>
            {
                game.Name,
                _settings.CommandLineOptions,
                "-screen",
                $"\"{_screen.DeviceName}\""
            };

            // Enable in-game titles if required
            if (_settings.LayoutSettings.InGameTitles.Enabled)
            {
                var artPath = _layoutBuilder.EnsureLayout(game, _screen.Bounds.Width, _screen.Bounds.Height);
                arguments.Add("-artpath");
                arguments.Add(artPath);
            }

            try
            {
                // Start MAME
                _mameProcess = _invoker.Run(false, arguments.ToArray());
                _mameProcess.Exited += OnMameExited;
                _mameProcess.Start();

                Log.Debug("MAME started; pid: {pid}", _mameProcess.Id);

                OnGameStarted?.Invoke(this, null);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unable to start game");
                _cancellationTokenSource.Cancel();

                throw;
            }
        }

        public virtual void Dispose(bool disposing)
        {
            if (!disposing || !_initialised) return;

            // Stop MAME and wait for it to terminate
            if (_mameProcess != null && !_mameProcess.HasExited) _invoker.Stop(_mameProcess);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~GamePlayManager() => Dispose(false);
    }
}