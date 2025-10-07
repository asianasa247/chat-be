namespace ManageEmployee.DataTransferObject.WeeklyScheduleModels
{
    public class WeeklyScheduleDetailModel
    {
        public int Id { get; set; }
        public int WeeklyScheduleId { get; set; }
        public DateTime Date { get; set; }
        public string Region { get; set; }
        public string ProvinceName { get; set; }
        public string Planning { get; set; }
        public string Note { get; set; }
    }
}
