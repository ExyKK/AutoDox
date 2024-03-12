using AutoDox.UI.Core;

namespace AutoDox.UI.ViewModels
{
    class MainViewModel : ObservableObject
    {
        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand ConfigurationViewCommand { get; set; }
        public RelayCommand SettingsViewCommand { get; set; }
        public RelayCommand ChangeThemeCommand { get; set; }

        public HomeViewModel HomeVM { get; set; }
        public ConfigurationViewModel ConfigurationVM { get; set; }
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
