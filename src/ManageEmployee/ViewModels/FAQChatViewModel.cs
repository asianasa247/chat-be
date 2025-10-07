using ManageEmployee.DataTransferObject.FAQ_AIchat;

namespace ManageEmployee.ViewModels
{
    public class FAQChatViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public string Department { get; set; }
        public string FirstTopic { get; set; }
        public List<FAQ_AIchatDetailModel> ChatDetails { get; set; } = new List<FAQ_AIchatDetailModel>();
    }
}
