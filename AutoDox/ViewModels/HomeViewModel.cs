using AutoDox.UI.Core;
using AutoDox.UI.Models;
using System;

namespace AutoDox.UI.ViewModels
{
    class HomeViewModel : ObservableObject
    {
        public RelayCommand BrowseDestinationDirectoryCommand { get; set; }
        public RelayCommand ReadFromDeviceCommand { get; set; }
        public RelayCommand ReadGitHubCommand { get; set; }

        private DiagramGeneratorManager _diagramGenerator;

        public static event EventHandler DestinationDirectoryChanged;
        private static object _destinationDirectory;
        public static object DestinationDirectory
        {
            get { return _destinationDirectory; }
            set
            {
                _destinationDirectory = value;
                DestinationDirectoryChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        public static event EventHandler SvgColorChanged;
        private static object _svgColor;
        public static object SvgColor
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

            SvgColor = ThemeChanger.GetSvgColor();

            BrowseDestinationDirectoryCommand = new RelayCommand(obj =>
            {
                DestinationDirectory = ExplorerDialog.SelectFolder();
            });

            ReadFromDeviceCommand = new RelayCommand(obj =>
            {
                _diagramGenerator.Run(DestinationDirectory);
            },
            (obj) => !string.IsNullOrEmpty((string)DestinationDirectory));

            ReadGitHubCommand = new RelayCommand(obj =>
            {
                
            },
            (obj) => !string.IsNullOrEmpty((string)DestinationDirectory));
        }
    }
}
