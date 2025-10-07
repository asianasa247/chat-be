using Newtonsoft.Json;

namespace ManageEmployee.Models.ZaloModel
{
    public class ZaloResp<T>
    {
        [JsonProperty("data")]
        public T Data { get; set; }

        [JsonProperty("error")]
        public long Error { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public class DataZaloResp
    {
        [JsonProperty("total")]
        public long Total { get; set; }

        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("offset")]
        public long Offset { get; set; }

    }
    public class DataZaloUserResp: DataZaloResp
    {
        [JsonProperty("users")]
        public List<DataZaloUser> Users { get; set; } = new List<DataZaloUser>();
    }
    public  class DataZaloUser
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }
    }
}
