using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShouldIStreamThis.Handlers;
using ShouldIStreamThis.Model;
using ShouldIStreamThis.Service;
using ShouldIStreamThis.Windows;
using System.Windows;

namespace ShouldIStreamThis.ViewModel
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly TomlConfigurationService _tomlConfigurationService = new();
        private readonly TwitchApiService _twitchService = new();
        private readonly ToStreamOrNotToStreamHandler _toStreamOrNotToStreamHandler = new();

        private string _clientId = string.Empty;
        private string _clientSecret = string.Empty;

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private string _searchGame = string.Empty;

        [ObservableProperty]
        private string _totalViewers = "Waiting";

        [ObservableProperty]
        private string _totalChannels = "Waiting";

        [ObservableProperty]
        private string _statusText = "Ready";

        [ObservableProperty]
        private bool _isLoading = false;

        [ObservableProperty]
        private string _goOrNoGoColor = "#262626";

        public MainWindowViewModel()
        {
            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            try
            {
                _clientId = _tomlConfigurationService.ReadTOMLField<string>("ClientId");
                _clientSecret = _tomlConfigurationService.ReadTOMLField<string>("ClientSecret");
            }
            catch
            {
            }
        }

        [RelayCommand]
        private async Task FetchMetricsAsync()
        {
            if (IsLoading) return;

            LoadConfiguration();

            if (string.IsNullOrWhiteSpace(_clientId) || string.IsNullOrWhiteSpace(_clientSecret))
            {
                StatusText = "Error: Please configure Client ID & Secret in Settings.";
                return;
            }

            IsLoading = true;
            StatusText = $"Fetching metrics for '{SearchGame}'...";

            try
            {
                TwitchMatricsModel metrics = await _twitchService.GetGameMetricsAsync(SearchGame, _clientId, _clientSecret);
                TotalViewers = $"Total Viewers: {metrics.TotalViewers.ToString("N0")}";
                TotalChannels = $"Active Channels: {metrics.ActiveChannels.ToString("N0")}";
                StatusText = $"Updated at {metrics.Timestamp:HH:mm:ss}";
                GoOrNoGoColor = _toStreamOrNotToStreamHandler.DetermineStreamingDecision((int)metrics.TotalViewers, metrics.ActiveChannels);
            }
            catch (Exception ex)
            {
                StatusText = $"Error: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void OpenSettingsWindow(Window ownerWindow)
        {
            SettingsWindow settingsWindow = new()
            {
                Owner = ownerWindow
            };

            settingsWindow.ShowDialog();

            LoadConfiguration();
        }
    }
}