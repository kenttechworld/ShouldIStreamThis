using ShouldIStreamThis.Model;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using static ShouldIStreamThis.Model.JsonDataModel;

namespace ShouldIStreamThis.Service
{

    public class TwitchApiService
    {
        private static readonly HttpClient HttpClient = new();
        private string _accessToken = string.Empty;

        public async Task<TwitchMatricsModel> GetGameMetricsAsync(string gameName, string clientId, string clientSecret)
        {
            if (string.IsNullOrEmpty(_accessToken))
            {
                _accessToken = await GetAccessTokenAsync(clientId, clientSecret);
            }

            HttpClient.DefaultRequestHeaders.Clear();
            HttpClient.DefaultRequestHeaders.Add("Client-ID", clientId);
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

            string? gameId = await GetGameIdAsync(gameName);
            if (string.IsNullOrEmpty(gameId))
            {
                throw new Exception($"Game '{gameName}' was not found on Twitch.");
            }

            var (totalViewers, totalChannels) = await FetchStreamMetricsAsync(gameId);

            return new TwitchMatricsModel
            {
                TotalViewers = totalViewers,
                ActiveChannels = totalChannels,
                Timestamp = DateTime.Now
            };
        }

        private async Task<string> GetAccessTokenAsync(string clientId, string clientSecret)
        {
            Dictionary<string, string> values = new()
            {
            { "client_id", clientId },
            { "client_secret", clientSecret },
            { "grant_type", "client_credentials" }
        };

            FormUrlEncodedContent content = new(values);
            HttpResponseMessage response = await HttpClient.PostAsync("https://id.twitch.tv/oauth2/token", content);
            response.EnsureSuccessStatusCode();

            string jsonString = await response.Content.ReadAsStringAsync();
            using JsonDocument doc = JsonDocument.Parse(jsonString);
            return doc.RootElement.GetProperty("access_token").GetString()!;
        }

        private async Task<string?> GetGameIdAsync(string gameName)
        {
            HttpResponseMessage response = await HttpClient.GetAsync($"https://api.twitch.tv/helix/games?name={Uri.EscapeDataString(gameName)}");
            response.EnsureSuccessStatusCode();

            string jsonString = await response.Content.ReadAsStringAsync();
            TwitchGameResponse? gameResult = JsonSerializer.Deserialize<TwitchGameResponse>(jsonString);

            return gameResult?.Data?.FirstOrDefault()?.Id;
        }

        private async Task<(long TotalViewers, int TotalChannels)> FetchStreamMetricsAsync(string gameId)
        {
            long totalViewers = 0;
            int totalChannels = 0;
            string? cursor = null;

            do
            {
                string url = $"https://api.twitch.tv/helix/streams?game_id={gameId}&first=100";
                if (!string.IsNullOrEmpty(cursor))
                {
                    url += $"&after={cursor}";
                }

                HttpResponseMessage response = await HttpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string jsonString = await response.Content.ReadAsStringAsync();
                TwitchStreamResponse? streamResult = JsonSerializer.Deserialize<TwitchStreamResponse>(jsonString);

                if (streamResult?.Data != null && streamResult.Data.Any())
                {
                    totalChannels += streamResult.Data.Count;
                    totalViewers += streamResult.Data.Sum(s => (long)s.ViewerCount);
                    cursor = streamResult.Pagination?.Cursor;
                }
                else
                {
                    break;
                }

            } while (!string.IsNullOrEmpty(cursor));

            return (totalViewers, totalChannels);
        }
    }
}
