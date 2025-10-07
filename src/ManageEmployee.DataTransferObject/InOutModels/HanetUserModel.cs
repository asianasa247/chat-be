
using ManageEmployee.Entities.Enumerations.HanetEnums;

namespace ManageEmployee.DataTransferObject.InOutModels
{
    public class HanetUserModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public List<int> UserIds { get; set; }
        public int PlaceId { get; set; }
        public InoutTypeHanet Type { get; set; }
        public string Note { get; set; }

    }
}
