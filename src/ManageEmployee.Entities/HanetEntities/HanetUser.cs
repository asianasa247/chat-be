using ManageEmployee.Entities.BaseEntities;
using ManageEmployee.Entities.Enumerations.HanetEnums;

namespace ManageEmployee.Entities.HanetEntities
{
    public class HanetUser :BaseEntityCommon
    {
        public int Id { get; set; }
        public string Code { get; set; }

        public string Name { get; set; }
        public string UserIds {  get; set; }
        public int PlaceId {  get; set; }
        public InoutTypeHanet Type {  get; set; }
        public string Note {  get; set; }
    }
}
