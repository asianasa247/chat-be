using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.ProcedureEntities.WeeklyScheduleEntities
{
    public class WeeklySchedule : BaseProcedureEntityCommon
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public DateTime FromAt { get; set; }
        public DateTime ToAt { get; set; }
        public string Note { get; set; }
    }
}
