using AutoDox.UI.Core;
using AutoDox.UI.Models;

namespace AutoDox.UI.ViewModels
{
    class ConfigurationViewModel : ObservableObject
    {
        public RelayCommand BrowseDestinationDirectoryCommand { get; set; }

        private object _destinationDirectory;
        public object DestinationDirectory
        {
            get { return _destinationDirectory; }
            set
            {
                _destinationDirectory = value;
                HomeViewModel.DestinationDirectory = value;
                OnPropertyChanged();
            }
        }

        public ConfigurationViewModel()
        {
            DestinationDirectory = HomeViewModel.DestinationDirectory;

            BrowseDestinationDirectoryCommand = new RelayCommand(obj =>
            {
                DestinationDirectory = ExplorerDialog.SelectFolder();
            });
        }
    }
}
