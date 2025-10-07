namespace ManageEmployee.DataTransferObject.FAQ_AIchat
{
    public class FAQ_AIchatDetailModel
    {
        public int FAQ_AIchatId { get; set; }
        public string Question { get; set; } = "";
        public string Answer { get; set; } = "";
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public string Topic { get; set; }
    }
}
