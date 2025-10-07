namespace ManageEmployee.DataTransferObject.Cultivation
{
    public class PlantingTypeModel
    {
        public int Id { get; set; }
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public int Category { get; set; } // map từ enum PlantingTypeCategory
    }
}
