using AutoDox.Properties;
using AutoDox.UI.ViewModels;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace AutoDox.UI.Models
{
    internal class ConfigurationManager(ConfigurationViewModel configurationVM)
    {
        public void SaveConfiguration()
        {
            Config.Default.DestinationDirectory = configurationVM.DestinationDirectory;
            Config.Default.ExcludedPaths = configurationVM.ExcludedPaths;
            Config.Default.InputMode = configurationVM.InputMode;
            Config.Default.SelectedModifierItems = string.Join(",", configurationVM.SelectedModifierItems);
            Config.Default.PublicMembersParameter = configurationVM.PublicMembersParameter;
            Config.Default.AssociationsParameter = configurationVM.AssociationsParameter;
            Config.Default.AllInOneParameter = configurationVM.AllInOneParameter;
            Config.Default.AttributeParameter = configurationVM.AttributeParameter;
            Config.Default.ExcludeTagsParameter = configurationVM.ExcludeTagsParameter;
            Config.Default.Save();
        }

        public void LoadConfiguration()
        {
            configurationVM.DestinationDirectory = Config.Default.DestinationDirectory;
            configurationVM.ExcludedPaths = Config.Default.ExcludedPaths;
            configurationVM.InputMode = Config.Default.InputMode;
            configurationVM.SelectedModifierItems = Config.Default.SelectedModifierItems.Split(',').ToList();
            configurationVM.PublicMembersParameter = Config.Default.PublicMembersParameter;
            configurationVM.AssociationsParameter = Config.Default.AssociationsParameter;
            configurationVM.AllInOneParameter = Config.Default.AllInOneParameter;
            configurationVM.AttributeParameter = Config.Default.AttributeParameter;
            configurationVM.ExcludeTagsParameter = Config.Default.ExcludeTagsParameter;
        }

        public void SetSelectedModifierItems()
        {
            Config.Default.SelectedModifierItems = string.Join(",", configurationVM.SelectedModifierItems);
            Config.Default.Save();
        }

        public static Dictionary<string, object> GetConfiguration()
        {
            IEnumerator settingsEnumerator = Config.Default.PropertyValues.GetEnumerator();  
            Dictionary<string, object> parameters = new();
            
            while (settingsEnumerator.MoveNext())
            {
                string key = ((SettingsPropertyValue)settingsEnumerator.Current).Name;
                object value = ((SettingsPropertyValue)settingsEnumerator.Current).PropertyValue;
                parameters[key] = value;
            }

            return parameters;
        }

        public static string GetDestinationDirectory()
        {
            return Config.Default.DestinationDirectory;
        }

        public static void SetDestinationDirectory(string destinationDirectory)
        {
            Config.Default.DestinationDirectory = destinationDirectory;
            Config.Default.Save();
        }
    }
}
