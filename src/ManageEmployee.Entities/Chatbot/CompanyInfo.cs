using System.Text.Json;
using System.Text.Json.Serialization;

namespace ManageEmployee.Entities.Chatbot
{
    public class CompanyInfo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("slogan")]
        public string? Slogan { get; set; }

        [JsonPropertyName("hotline")]
        public string? Hotline { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("address")]
        public string? Address { get; set; }

        [JsonPropertyName("working_hours")]
        public string? WorkingHours { get; set; }

        [JsonPropertyName("services")]
        public List<string> Services { get; set; } = new();

        [JsonPropertyName("faq")]
        public List<FaqItem> Faq { get; set; } = new();
    }

    public class FaqItem
    {
        // Trong file JSON, "q" có thể là string hoặc mảng string → dùng converter
        [JsonPropertyName("q")]
        [JsonConverter(typeof(StringOrArrayConverter))]
        public List<string> Triggers { get; set; } = new();

        [JsonPropertyName("a")]
        public string Answer { get; set; } = "";
    }

    /// <summary>
    /// Converter cho property có thể là "string" hoặc "string[]".
    /// </summary>
    public sealed class StringOrArrayConverter : JsonConverter<List<string>>
    {
        public override List<string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var list = new List<string>();
            if (reader.TokenType == JsonTokenType.String)
            {
                list.Add(reader.GetString() ?? "");
                return list;
            }
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                {
                    if (reader.TokenType == JsonTokenType.String)
                        list.Add(reader.GetString() ?? "");
                }
                return list;
            }
            // kiểu khác → bỏ qua
            return list;
        }

        public override void Write(Utf8JsonWriter writer, List<string> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var s in value) writer.WriteStringValue(s);
            writer.WriteEndArray();
        }
    }
}
