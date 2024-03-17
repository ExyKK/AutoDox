using AutoDox.UI.Helpers;
using AutoDox.UI.Core;
using AutoDox.UI.Models;
using System;

namespace AutoDox.UI.ViewModels
{
    class HomeViewModel : ObservableObject
    {
        private string _destinationDirectory;
        private static string _svgColor;
        private DiagramGeneratorManager _diagramGenerator;

        public RelayCommand BrowseDestinationDirectoryCommand { get; set; }
        public RelayCommand ReadFromDeviceCommand { get; set; }
        public RelayCommand ReadGitHubCommand { get; set; }

        public string DestinationDirectory
        {
            get { return _destinationDirectory; }
            set
            {
                _destinationDirectory = value;
                ConfigurationManager.SetDestinationDirectory(value);
                OnPropertyChanged();
            }
        }

        public static event EventHandler SvgColorChanged;
        public static string SvgColor
        {
            get { return _svgColor; }
            set
            {
                _svgColor = value;
                SvgColorChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        public HomeViewModel() 
        {
            _diagramGenerator = new DiagramGeneratorManager();

            DestinationDirectory = ConfigurationManager.GetDestinationDirectory();
            SvgColor = ThemeChanger.GetSvgColor();

            BrowseDestinationDirectoryCommand = new RelayCommand(obj =>
            {
                DestinationDirectory = ExplorerDialog.SelectFolder();
            });

            ReadFromDeviceCommand = new RelayCommand(obj =>
            {
                _diagramGenerator.Run(DestinationDirectory);
            },
            (obj) => !string.IsNullOrEmpty(DestinationDirectory));

            ReadGitHubCommand = new RelayCommand(obj =>
            {
                
            },
            (obj) => !string.IsNullOrEmpty(DestinationDirectory));
        }
    }
}
