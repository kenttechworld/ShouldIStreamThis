using System.Text.Json.Serialization;

namespace ShouldIStreamThis.Model
{
    internal class JsonDataModel
    {
        public class TwitchGameResponse
        {
            [JsonPropertyName("data")]
            public List<GameData>? Data { get; set; }
        }

        public class GameData
        {
            [JsonPropertyName("id")]
            public string Id { get; set; } = string.Empty;

            [JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;
        }

        public class TwitchStreamResponse
        {
            [JsonPropertyName("data")]
            public List<StreamData>? Data { get; set; }

            [JsonPropertyName("pagination")]
            public PaginationData? Pagination { get; set; }
        }

        public class StreamData
        {
            [JsonPropertyName("viewer_count")]
            public int ViewerCount { get; set; }
        }

        public class PaginationData
        {
            [JsonPropertyName("cursor")]
            public string? Cursor { get; set; }
        }
    }
}
