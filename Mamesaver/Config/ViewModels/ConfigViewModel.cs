using System;
using System.Windows;
using System.Windows.Input;
using Mamesaver.Models.Configuration;
using Mamesaver.Services.Configuration;
using Prism.Commands;

namespace Mamesaver.Config.ViewModels
{
    public class ConfigViewModel : ViewModel
    {
        private Settings _settings;
        private readonly GeneralSettingsStore _generalSettingsStore;

        public static event ResetToDefaultsEventHander ResetToDefaults;
        public static event EventHandler Save;

        public delegate void ResetToDefaultsEventHander(object sender, ResetToDefaultsEventArgs e);

        public ConfigViewModel(
            Settings settings,
            GeneralSettingsStore generalSettingsStore)
        {
            _settings = settings;
            _generalSettingsStore = generalSettingsStore;
        }

        public ICommand OkClick => new DelegateCommand(SaveAndClose);
        public ICommand CancelClick => new DelegateCommand(Close);

        public ICommand ResetToDefaultsClick => new DelegateCommand(() =>
        {
            _settings = new Settings();
            ResetToDefaults?.Invoke(this, new ResetToDefaultsEventArgs(_settings));
        });

        private void SaveAndClose()
        {
            _generalSettingsStore.Save(_settings);
            Save?.Invoke(this, EventArgs.Empty);

            Close();
        }

        private static void Close() => Application.Current.Shutdown();
    }
}