using AutoDox.UI.Helpers;
using AutoDox.UI.Core;
using System;

namespace AutoDox.UI.ViewModels
{
    class SettingsViewModel : ObservableObject
    {
        private object _language;
        private static string _theme;

        public RelayCommand ChangeThemeCommand { get; set; }

        public object Language
        {
            get { return _language; }
            set
            {
                _language = value;
                OnPropertyChanged();
            }
        }

        public static event EventHandler ThemeChanged;
        public static string Theme
        {
            get { return _theme; }
            set
            {
                _theme = value;
                ThemeChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        public SettingsViewModel()
        {
            Theme = ThemeChanger.GetTheme();

            ChangeThemeCommand = new RelayCommand(obj =>
            {
                ThemeChanger.Change();
            });
        }
    }
}
