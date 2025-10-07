namespace ManageEmployee.DataTransferObject.WeeklyScheduleModels
{
    public class WeeklyScheduleModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public DateTime FromAt { get; set; }
        public DateTime ToAt { get; set; }
        public string Note { get; set; }
        public string ProcedureNumber { get; set; }
        public string ProcedureStatusName { get; set; }
        public bool IsSave { get; set; }
        public List<WeeklyScheduleDetailModel> Items { get; set; }
    }
}