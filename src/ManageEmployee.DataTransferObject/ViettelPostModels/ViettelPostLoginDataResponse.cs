using System.Text.Json.Serialization;

namespace ManageEmployee.DataTransferObject.ViettelPostModels;

public class ViettelPostLoginDataResponse
{
    [JsonPropertyName("userId")]
    public long UserId { get; set; }

    [JsonPropertyName("token")]
    public string Token { get; set; }

    [JsonPropertyName("partner")]
    public long Partner { get; set; }

    [JsonPropertyName("phone")]
    public string Phone { get; set; }

    [JsonPropertyName("postcode")]
    public string Postcode { get; set; }

    [JsonPropertyName("expired")]
    public long Expired { get; set; }

    [JsonPropertyName("encrypted")]
    public string Encrypted { get; set; }

    [JsonPropertyName("source")]
    public int Source { get; set; }
}
