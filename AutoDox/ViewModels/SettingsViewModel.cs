using AutoDox.UI.Helpers;
using AutoDox.UI.Core;
using System;
using System.Collections.Generic;

namespace AutoDox.UI.ViewModels
{
    class SettingsViewModel : ObservableObject
    {
        private List<string> _languages =
        [
            "Русский",
            "English",
        ];
        private string _currentLanguage;
        private static string _theme;

        public RelayCommand ChangeThemeCommand { get; set; }
        public RelayCommand ChangeLanguageCommand { get; set; }

        public List<string> Languages
        {
            get { return _languages; }
            set 
            { 
                _languages = value;
                OnPropertyChanged();
            }
        }

        public string CurrentLanguage
        {
            get { return _currentLanguage; }
            set
            {
                _currentLanguage = value;
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
            CurrentLanguage = LanguageChanger.GetLanguage();

            ChangeThemeCommand = new RelayCommand(obj =>
            {
                ThemeChanger.Change();
            });

            ChangeLanguageCommand = new RelayCommand(obj =>
            {
                LanguageChanger.Change();
            });
        }
    }
}
