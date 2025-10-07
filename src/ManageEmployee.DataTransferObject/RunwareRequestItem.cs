using System.Text.Json.Serialization;

namespace ManageEmployee.DataTransferObject
{
        public class RunwareRequestItem
        {
            [JsonPropertyName("taskType")]
            public string TaskType { get; set; } = null!;

            [JsonPropertyName("apiKey")]
            public string? ApiKey { get; set; }

            [JsonPropertyName("taskUUID")]
            public string? TaskUUID { get; set; }

            [JsonPropertyName("positivePrompt")]
            public string? PositivePrompt { get; set; }

            [JsonPropertyName("width")]
            public int? Width { get; set; }

            [JsonPropertyName("height")]
            public int? Height { get; set; }

            [JsonPropertyName("model")]
            public string? Model { get; set; }

            [JsonPropertyName("numberResults")]
            public int? NumberResults { get; set; }
        }
    }

