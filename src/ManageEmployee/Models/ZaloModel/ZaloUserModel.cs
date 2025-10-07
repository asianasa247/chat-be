using Newtonsoft.Json;

namespace ManageEmployee.Models.ZaloModel
{
    public class ZaloUserRespModel
    {
        [JsonProperty("user_id")]
        public long UserId { get; set; }
        [JsonProperty("user_id_by_app")]
        public long UserIdByApp { get; set; }
        [JsonProperty("user_external_id")]
        public string UserExternalId { get; set; }
        [JsonProperty("display_name")]
        public string DisplayName { get; set; }
        [JsonProperty("user_alias")]
        public string UserAlias { get; set; }
        [JsonProperty("is_sensitive")]
        public bool IsSensitive { get; set; }
        [JsonProperty("user_last_interaction_date")]
        public string UserLastInteractionDate { get; set; }
        [JsonProperty("user_is_follower")]
        public bool UserIsFollower { get; set; }
        [JsonProperty("avatar")]
        public string Avatar { get; set; }
        [JsonProperty("avatars")]
        public Dictionary<string,string> Avatars { get; set; }
        [JsonProperty("tags_and_notes_info")]
        public object TagsAndNotesInfo { get; set; }
        [JsonProperty("shared_info")]
        public object SharedInfo { get; set; }
    }
}
