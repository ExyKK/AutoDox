using AutoDox.UI.Core;
using HandyControl.Themes;

namespace AutoDox.UI.ViewModels
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
                    ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
                    HomeViewModel.SvgColor = "#FFFFFF";
                }
                else
                {
                    ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
                    HomeViewModel.SvgColor = "#000000";
                }
            });
        }
    }
}
