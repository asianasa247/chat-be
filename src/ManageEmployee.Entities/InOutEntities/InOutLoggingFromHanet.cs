using ManageEmployee.Entities.Enumerations.HanetEnums;

namespace ManageEmployee.Entities.InOutEntities
{
    public class InOutLoggingFromHanet
    {
        public int Id { get; set; }
        public DateTime? TimeIn { get; set; }
        public DateTime? TimeOut { get; set; }
        public PersonTypeHanet PersonType { get; set; }
    }
}
