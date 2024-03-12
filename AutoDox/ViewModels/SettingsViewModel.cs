using AutoDox.UI.Core;
using System;

namespace AutoDox.UI.ViewModels
{
    class SettingsViewModel : ObservableObject
    {
        public RelayCommand ChangeThemeCommand { get; set; }

        private object _language;
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
        private static string _theme;
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
