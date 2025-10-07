using ManageEmployee.Entities.Enumerations.HanetEnums;

namespace ManageEmployee.DataTransferObject.InOutModels
{
    public class HanetModel
    {
        public string action_type { get; set; }
        public string detected_image_url { get; set; }
        public string data_type { get; set; }
        public DateTime date { get; set; }
        public string deviceID { get; set; }
        public string deviceName { get; set; }
        public string hash { get; set; }
        public string id { get; set; }
        public string keycode { get; set; }
        public string placeID { get; set; }
        public string placeName { get; set; }
        public double time { get; set; }
        public string personID { get; set; }
        public string personName { get; set; }
        public string personTitle { get; set; }
        public string mask { get; set; }
        public PersonTypeHanet personType { get; set; }
    }
}
