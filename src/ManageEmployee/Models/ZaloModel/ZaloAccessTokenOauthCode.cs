namespace ManageEmployee.Models.ZaloModel
{
    public class ZaloAccessTokenOauthCodeReq
    {
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public string Code { get; set; }
    }
}
