using System.Text.Json.Serialization;

namespace ManageEmployee.DataTransferObject.ViettelPostModels;

public class ViettelPostResponseApiResponse<T>
{
    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("error")]
    public bool Error { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("data")]
    public T Data { get; set; }
}
