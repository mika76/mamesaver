using System;

namespace Mamesaver.HotKeys
{
    /// <inheritdoc />
    /// <summary>
    ///     Payload for hotkey events
    /// </summary>
    public class HotKeyEventArgs : EventArgs
    {
        public HotKeyEventArgs(HotKey hotKey) => HotKey = hotKey;
        public HotKey HotKey { get; }
    }
}