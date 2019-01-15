using System;
using Mamesaver.Models.Configuration;

namespace Mamesaver.Config.ViewModels
{
    public class ResetToDefaultsEventArgs : EventArgs
    {
        public ResetToDefaultsEventArgs(Settings settings)
        {
            Settings = settings;
        }

        public Settings Settings { get; }
    }
}