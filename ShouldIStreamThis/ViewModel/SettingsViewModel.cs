using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShouldIStreamThis.Handlers;
using ShouldIStreamThis.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace ShouldIStreamThis.ViewModel
{
    partial class SettingsViewModel : ObservableObject
    {
        TomlConfigurationService tomlConfigurationService = new();

        [ObservableProperty]
        private string _clientId = string.Empty;

        [ObservableProperty]
        private string _clientSecret = string.Empty;

        [ObservableProperty]
        private string _buttonText = "Save";

        public SettingsViewModel()
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            ClientId = tomlConfigurationService.ReadTOMLField<string>("ClientId");
            ClientSecret = tomlConfigurationService.ReadTOMLField<string>("ClientSecret");
        }

        [RelayCommand]
        private void SaveSettings(Window window)
        {
            tomlConfigurationService.UpdateTOMLField("ClientId", ClientId);
            tomlConfigurationService.UpdateTOMLField("ClientSecret", ClientSecret);
            ButtonText = "Saved!";
        }
    }
}
