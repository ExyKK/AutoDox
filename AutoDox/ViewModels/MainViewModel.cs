using AutoDox.UI.Helpers;
using AutoDox.UI.Core;
using System;

namespace AutoDox.UI.ViewModels
{
    class MainViewModel : ObservableObject
    {
        private static object _currentView;

        public HomeViewModel HomeVM { get; set; }
        public ConfigurationViewModel ConfigurationVM { get; set; }
        public SettingsViewModel SettingsVM { get; set; }

        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand ConfigurationViewCommand { get; set; }
        public RelayCommand SettingsViewCommand { get; set; }
        public RelayCommand ChangeThemeCommand { get; set; }

        public static event EventHandler CurrentViewChanged;
        public static object CurrentView 
        {
            get { return _currentView; }
            set 
            { 
                _currentView = value;
                CurrentViewChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        public MainViewModel() 
        {
            HomeVM = new HomeViewModel();
            ConfigurationVM = new ConfigurationViewModel();
            SettingsVM = new SettingsViewModel();

            CurrentView = HomeVM;

            HomeViewCommand = new RelayCommand(obj =>
            {
                CurrentView = HomeVM;
            });

            ConfigurationViewCommand = new RelayCommand(obj =>
            {
                CurrentView = ConfigurationVM;
            });

            SettingsViewCommand = new RelayCommand(obj =>
            {
                CurrentView = SettingsVM;
            });

            ChangeThemeCommand = new RelayCommand(obj =>
            {
                ThemeChanger.Change();
            });
        }
    }
}
