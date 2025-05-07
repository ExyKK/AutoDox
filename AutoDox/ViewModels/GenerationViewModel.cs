using AutoDox.UI.Core;
using AutoDox.UI.Models;
using System.Diagnostics;

namespace AutoDox.UI.ViewModels
{
    class GenerationViewModel : ObservableObject
    {
        private DiagramGeneratorManager _diagramGenerator;

        public AsyncRelayCommand OnWindowLoadedCommand { get; set; }
        public RelayCommand ReturnHomeCommand { get; set; }
        public RelayCommand OpenDestinationDirectoryCommand { get; set; }

        public DiagramGeneratorManager DiagramGenerator
        {
            get { return _diagramGenerator; }
            set
            {
                _diagramGenerator = value;
                OnPropertyChanged();
            }
        }

        public GenerationViewModel()
        {
            _diagramGenerator = new DiagramGeneratorManager();
            
            OnWindowLoadedCommand = new AsyncRelayCommand(async obj =>
            {
                if (!await _diagramGenerator.Run())
                {
                    MainViewModel.CurrentView = new HomeViewModel();
                }
            });

            ReturnHomeCommand = new RelayCommand(obj =>
            {
                MainViewModel.CurrentView = new HomeViewModel();
            });

            OpenDestinationDirectoryCommand = new RelayCommand(obj =>
            {
                Process.Start("explorer.exe", ConfigurationManager.GetDestinationDirectory());                
            });

        }
    }
}
