using AutoDox.UI.Core;
using AutoDox.UI.Models;
using System.Collections.Generic;

namespace AutoDox.UI.ViewModels
{
    class ConfigurationViewModel : ObservableObject
    {
        private string _destinationDirectory;
        private string _excludedPaths;
        private string _inputMode;
        private List<string> _modifierItems =
        [
            "public",
            "internal",
            "protected internal",
            "protected",
            "private"
        ];
        private List<string> _selectedModifierItems;
        private bool _publicMembersParameter;
        private bool _associationsParameter;
        private bool _allInOneParameter;
        private bool _attributeParameter;
        private bool _excludeTagsParameter;

        private readonly ConfigurationManager _configManager;

        public RelayCommand BrowseDestinationDirectoryCommand { get; set; }
        public RelayCommand ChangeParameterCommand { get; set; }        

        public string DestinationDirectory
        {
            get { return _destinationDirectory; }
            set
            {
                _destinationDirectory = value;
                OnPropertyChanged();
                if (DestinationDirectory != null && SelectedModifierItems != null)
                    _configManager.SaveConfiguration();
            }
        }

        public string ExcludedPaths
        {
            get { return _excludedPaths; }
            set
            {
                _excludedPaths = value;
                OnPropertyChanged();
                if (ExcludedPaths != null && SelectedModifierItems != null)
                    _configManager.SaveConfiguration();
            }
        }

        public string InputMode
        {
            get { return _inputMode; }
            set
            {
                _inputMode = value;
                OnPropertyChanged();
            }
        }

        public List<string> ModifierItems
        {
            get { return _modifierItems; }
            set
            {
                _modifierItems = value;
                OnPropertyChanged();
            }
        }

        public List<string> SelectedModifierItems
        {
            get { return _selectedModifierItems; }
            set
            {
                _selectedModifierItems = value;                
                OnPropertyChanged();
                if (SelectedModifierItems != null)
                    _configManager.SetSelectedModifierItems();
            }
        }

        public bool PublicMembersParameter
        {
            get { return _publicMembersParameter; }
            set
            {
                _publicMembersParameter = value;
                OnPropertyChanged();
            }
        }

        public bool AssociationsParameter
        {
            get { return _associationsParameter; }
            set
            {
                _associationsParameter = value;
                OnPropertyChanged();
            }
        }

        public bool AllInOneParameter
        {
            get { return _allInOneParameter; }
            set
            {
                _allInOneParameter = value;
                OnPropertyChanged();
            }
        }

        public bool AttributeParameter
        {
            get { return _attributeParameter; }
            set
            {
                _attributeParameter = value;
                OnPropertyChanged();
            }
        }

        public bool ExcludeTagsParameter
        {
            get { return _excludeTagsParameter; }
            set
            {
                _excludeTagsParameter = value;
                OnPropertyChanged();
            }
        }

        public ConfigurationViewModel()
        {
            _configManager = new ConfigurationManager(this);
            _configManager.LoadConfiguration();

            BrowseDestinationDirectoryCommand = new RelayCommand(obj =>
            {
                DestinationDirectory = ExplorerDialog.SelectFolder();                
            });

            ChangeParameterCommand = new RelayCommand(obj =>
            {
                if (AllInOneParameter && InputMode == "Select_file")
                    AllInOneParameter = false;
                _configManager.SaveConfiguration();
            });
        }
    }
}
