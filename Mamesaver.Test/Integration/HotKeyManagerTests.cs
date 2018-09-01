using System;
using System.Threading;
using System.Windows.Forms;
using FluentAssertions;
using Mamesaver.Configuration.Models;
using Mamesaver.Hotkeys;
using Mamesaver.Test.Unit;
using NUnit.Framework;

namespace Mamesaver.Test.Integration
{
    [TestFixture]
    [TestOf(typeof(HotKeyManager))]
    public class HotKeyManagerTests : MamesaverTests
    {
        private HotKeyManager _manager;
        private TestActivityHook _activityHook;

        [SetUp]
        public void SetUp()
        {
            _activityHook = new TestActivityHook();
            _manager = new HotKeyManager(_activityHook, new Settings { HotKeys = true});
            _manager.Initialise();
        }


        public void UnhandledKeyManaged()
        {
            var resetEvent = new AutoResetEvent(false);
            _manager.UnhandledKeyPressed += (sender, args) => resetEvent.Set();

            _activityHook.SendKey(Keys.A);

            resetEvent
                .WaitOne(TimeSpan.FromSeconds(1))
                .Should()
                .BeTrue("unhandled hotkey event should have been fired");

        }

        [Test]
        public void HotKeyEventNotSendOnUnhandled()
        {
            var resetEvent = new AutoResetEvent(false);
            _manager.HotKeyPressed += (sender, args) => resetEvent.Set();

            _activityHook.SendKey(Keys.A);
            resetEvent
                .WaitOne(TimeSpan.FromSeconds(1))
                .Should()
                .BeFalse("event shouldn't have been fired");
        }

        [TestCase(Keys.Left, HotKey.PreviousGame)]
        [TestCase(Keys.Right, HotKey.NextGame)]
        [TestCase(Keys.Enter, HotKey.PlayGame)]
        [TestCase(Keys.Delete, HotKey.DeselectGame)]
        public void HotKeyManaged(Keys key, HotKey hotKey)
        {
            var resetEvent = new AutoResetEvent(false);
            _manager.HotKeyPressed += (sender, args) =>
            {
                resetEvent.Set();
                args.HotKey.Should().Be(hotKey);
            };

            _activityHook.SendKey(key);
            resetEvent.WaitOne(TimeSpan.FromSeconds(1));
        }

        [Test]
        [Description("Verifies that a hotkey is treated as unhandled if hotkeys are disabled")]
        public void HotKeyUnhandledIfHotKeysDisabled()
        {
            var manager = new HotKeyManager(_activityHook, new Settings { HotKeys = false });
            manager.Initialise();

            var hotKeyResetEvent = new AutoResetEvent(false);
            var unhandledResetEvent = new AutoResetEvent(false);

            manager.HotKeyPressed += (sender, args) => hotKeyResetEvent.Set();
            manager.UnhandledKeyPressed += (sender, args) => unhandledResetEvent.Set();

            _activityHook.SendKey(Keys.Left);

            hotKeyResetEvent
                .WaitOne(TimeSpan.FromSeconds(1))
                .Should()
                .BeFalse("event shouldn't have been fired");

            unhandledResetEvent
                .WaitOne(TimeSpan.FromSeconds(1))
                .Should()
                .BeTrue("hotkey should be treated as unhandled if hotkeys are disabled");
        }
    }
}