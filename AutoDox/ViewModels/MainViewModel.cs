using AutoDox.Core;
using HandyControl.Themes;
using System.Windows;

namespace AutoDox.ViewModels
{
    class MainViewModel : ObservableObject
    {
        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand SettingsViewCommand { get; set; }
        public RelayCommand ChangeThemeCommand { get; set; }

        public HomeViewModel HomeVM { get; set; }
        public SettingsViewModel SettingsVM { get; set; }

        private object _currentView;
        public object CurrentView 
        {
            get { return _currentView; }
            set 
            { 
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel() 
        {
            HomeVM = new HomeViewModel();
            SettingsVM = new SettingsViewModel();

            CurrentView = HomeVM;

            HomeViewCommand = new RelayCommand(obj =>
            {
                CurrentView = HomeVM;
            });

            SettingsViewCommand = new RelayCommand(obj =>
            {
                CurrentView = SettingsVM;
            });

            ChangeThemeCommand = new RelayCommand(obj =>
            {
                if (ThemeManager.Current.ApplicationTheme == ApplicationTheme.Light)
                {
                    ((App)Application.Current).UpdateTheme(ApplicationTheme.Dark);
                    HomeViewModel.SvgColor = "#FFFFFF";
                }
                else
                {
                    ((App)Application.Current).UpdateTheme(ApplicationTheme.Light);
                    HomeViewModel.SvgColor = "#000000";
                }
            });
        }
    }
}
