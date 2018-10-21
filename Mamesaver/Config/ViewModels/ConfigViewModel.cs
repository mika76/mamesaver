using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Mamesaver.Models.Configuration;
using Mamesaver.Properties;
using Mamesaver.Services.Configuration;
using Prism.Commands;

namespace Mamesaver.Config.ViewModels
{
    public class ConfigViewModel : ViewModel
    {
        private readonly Settings _settings;
        private readonly GeneralSettingsStore _generalSettingsStore;

       public static event EventHandler ResetToDefaults;
        public static event EventHandler Save;

        public ConfigViewModel(
            Settings settings,
            GeneralSettingsStore generalSettingsStore)
        {
            _settings = settings;
            _generalSettingsStore = generalSettingsStore;
        }

        public ICommand OkClick => new DelegateCommand(SaveAndClose);
        public ICommand CancelClick => new DelegateCommand(Close);

        public ICommand ResetToDefaultsClick => new DelegateCommand(() => ResetToDefaults?.Invoke(this, EventArgs.Empty));

        private void SaveAndClose()
        {
            _generalSettingsStore.Save(_settings);
            Save?.Invoke(this, EventArgs.Empty);

            Close();
        }

        private static void Close() => Application.Current.Shutdown();
    }
}