using AutoDox.Core;
using AutoDox.Models;
using HandyControl.Themes;
using System;

namespace AutoDox.ViewModels
{
    class HomeViewModel : ObservableObject
    {
        public RelayCommand BrowseDestinationDirectoryCommand { get; set; }
        public RelayCommand ReadFromDeviceCommand { get; set; }
        public RelayCommand ReadGitHubCommand { get; set; }

        private DiagramGenerator _diagramGenerator;
        private ExplorerDialog _explorerDialog;

        private object _destinationDirectory;
        public object DestinationDirectory
        {
            get { return _destinationDirectory; }
            set
            {
                _destinationDirectory = value;
                OnPropertyChanged();
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
            _explorerDialog = new ExplorerDialog();
            _diagramGenerator = new DiagramGenerator();

            if (ThemeManager.Current.ApplicationTheme == ApplicationTheme.Light)
                SvgColor = "#000000";
            else
                SvgColor = "#FFFFFF";

            BrowseDestinationDirectoryCommand = new RelayCommand(obj =>
            {
                DestinationDirectory = _explorerDialog.SelectFolder();
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
